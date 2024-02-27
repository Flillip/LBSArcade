using Microsoft.Xna.Framework;
using NvgSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace LBSArcade
{
    internal class SvgRenderer
    {
        private NvgContext context;

        private static Dictionary<char, Commands> charToCmd = new Dictionary<char, Commands>()
        {
            {'M', Commands.MoveTo},
            {'m', Commands.MoveToShift},
            {'L', Commands.LineTo},
            {'l', Commands.LineToShift},
            {'H', Commands.LineToHorizontal},
            {'h', Commands.LineToHorizontalShift},
            {'V', Commands.LineToVertical},
            {'v', Commands.LineToVerticalShift},
            {'C', Commands.Cubic},
            {'c', Commands.CubicShift},
            {'S', Commands.CubicSmooth},
            {'s', Commands.CubicSmoothShift},
            {'Q', Commands.Quadratic},
            {'q', Commands.QuadraticShift},
            {'T', Commands.QuadraticSmooth},
            {'t', Commands.QuadraticSmoothShift},
            {'A', Commands.Elliptical},
            {'a', Commands.EllipticalShift},
            {'Z', Commands.ClosePath},
            {'z', Commands.ClosePath},
        };
        private static char[] singleArgCmd = new char[]
        {
            'H', 'h', 'V', 'v'
        };

        public SvgRenderer()
        {
            this.context = new NvgContext(Game.Instance.GraphicsDevice);
        }

        public PathCommand[] ParsePath(string path)
        {
            List<PathCommand> pathCommands = new List<PathCommand>();

            Regex regex = new("([A-Za-z])");
            string[] split = regex.Split(path);
            List<Vector2> points = new();
            char command = '\0';

            for (int i = 0; i < split.Length; i++)
            {
                string cmd = split[i];

                if (cmd == "")
                    continue;

                if (char.IsLetter(cmd[0]))
                {
                    if (points.Count > 0)
                        pathCommands.Add(new PathCommand(charToCmd[command], points.ToArray()));

                    command = cmd[0];
                    points = new List<Vector2>();

                    if (charToCmd[command] == Commands.ClosePath)
                        pathCommands.Add(new PathCommand(charToCmd[command], new Vector2(0, 0)));

                }

                else
                {
                    cmd = cmd.Replace(',', ' ');
                    float[] splitVec = cmd.Split(' ').Where((el) => el.ToString() != "").Select((el) => Convert.ToSingle(el, CultureInfo.InvariantCulture)).ToArray();

                    if (singleArgCmd.Contains(command))
                    {
                        for (int j = 0; j < splitVec.Length; j++)
                        {
                            float x = splitVec[j];
                            float y = 0;

                            points.Add(new(x, y));
                        }

                        continue;
                    }

                    //else if (command.ToString().ToLower() == "a")
                    //{


                    //    continue;
                    //}

                    for (int j = 0; j < splitVec.Length; j += 2)
                    {
                        float x = splitVec[j];
                        float y = splitVec[j + 1];

                        points.Add(new(x, y));
                    }
                }
            }

            return pathCommands.ToArray();
        }

        public void Render(PathCommand[] pathCommands, Color color, Vector2 translate = default)
        {
            translate = translate == default ? new(0, 0) : translate;

            context.BeginFrame(Game.ScreenSize.X, Game.ScreenSize.Y, 1);

            context.Translate(translate.X, translate.Y);

            context.BeginPath();

            Vector2 position = new(0, 0);

            for (int i = 0; i < pathCommands.Length; i++)
            {
                PathCommand command = pathCommands[i];
                Vector2[] points = command.Points;
                Vector2 single = points[0];

                switch (command.Command)
                {
                    case Commands.MoveTo:
                        ExecuteFunction(points, (pos) =>
                        {
                            position = pos;
                            context.MoveTo(pos.X, pos.Y);
                        });
                        break;
                    case Commands.MoveToShift:
                        ExecuteFunction(points, (pos) =>
                        {
                            position += pos;
                            context.MoveTo(position.X, position.Y);
                        });
                        break;
                    case Commands.LineTo:
                        ExecuteFunction(points, (pos) =>
                        {
                            context.LineTo(pos.X, pos.Y);
                        });
                        break;
                    case Commands.LineToShift:
                        ExecuteFunction(points, (pos) =>
                        {
                            context.LineTo(position.X + pos.X, position.Y + pos.Y);
                        });
                        break;
                    case Commands.LineToHorizontal:
                        ExecuteFunction(points, (pos) =>
                        {
                            context.LineTo(pos.X, position.Y);
                        });
                        break;
                    case Commands.LineToHorizontalShift:
                        ExecuteFunction(points, (pos) =>
                        {
                            context.LineTo(position.X + pos.X, position.Y);
                        });
                        break;
                    case Commands.LineToVertical:
                        ExecuteFunction(points, (pos) =>
                        {
                            context.LineTo(position.X, pos.Y);
                        });
                        break;
                    case Commands.LineToVerticalShift:
                        ExecuteFunction(points, (pos) =>
                        {
                            context.LineTo(position.X, position.Y + pos.Y);
                        });
                        break;
                    case Commands.Cubic:
                    case Commands.CubicSmooth:
                        ExecuteFunction(points, (pos, pos2, pos3) =>
                        {
                            context.BezierTo(pos.X, pos.Y, pos2.X, pos2.Y, pos3.X, pos3.Y);
                        });
                        break;
                    case Commands.CubicShift:
                    case Commands.CubicSmoothShift:
                        ExecuteFunction(points, (pos, pos2, pos3) =>
                        {
                            context.BezierTo(position.X + pos.X, position.Y + pos.Y,
                                             position.X + pos2.X, position.Y + pos2.Y,
                                             position.X + pos3.X, position.Y + pos3.Y);
                        });
                        break;
                    case Commands.Quadratic:
                    case Commands.QuadraticSmooth:
                        ExecuteFunction(points, (pos, pos2) =>
                        {
                            context.QuadTo(pos.X, pos.Y, pos2.X, pos2.Y);
                        });
                        break;
                    case Commands.QuadraticShift:
                    case Commands.QuadraticSmoothShift:
                        ExecuteFunction(points, (pos, pos2) =>
                        {
                            context.QuadTo(position.X + pos.X, position.Y + pos.Y, position.X + pos2.X, position.Y + pos2.Y);
                        });
                        break;
                    case Commands.Elliptical:
                        throw new NotImplementedException("Elliptical is not implemented");
                    case Commands.EllipticalShift:
                        throw new NotImplementedException("Elliptical shift is not implemented");
                    case Commands.ClosePath:
                        context.ClosePath();
                        break;
                }
            }


            context.FillColor(color);
            context.Fill();


            context.EndFrame();
        }

        private void ExecuteFunction(Vector2[] points, Action<Vector2> function)
        {
            for (int i = 0; i < points.Length; i++)
                function(points[i]);
        }

        private void ExecuteFunction(Vector2[] points, Action<Vector2, Vector2> function)
        {
            for (int i = 0; i < points.Length; i += 2)
                function(points[i], points[i + 1]);
        }

        private void ExecuteFunction(Vector2[] points, Action<Vector2, Vector2, Vector2> function)
        {
            for (int i = 0; i < points.Length; i += 3)
                function(points[i], points[i + 1], points[i + 2]);
        }
    }

    internal struct PathCommand
    {
        public Commands Command;
        public Vector2[] Points;

        public PathCommand(Commands command, Vector2[] points)
        {
            this.Command = command;
            this.Points = points;
        }

        public PathCommand(Commands command, Vector2 point)
        {
            this.Command = command;
            this.Points = new Vector2[] { point };
        }
    }

    internal enum Commands
    {
        MoveTo,
        MoveToShift,
        LineTo,
        LineToShift,
        LineToHorizontal,
        LineToHorizontalShift,
        LineToVertical,
        LineToVerticalShift,
        Cubic,
        CubicShift,
        CubicSmooth,
        CubicSmoothShift,
        Quadratic,
        QuadraticShift,
        QuadraticSmooth,
        QuadraticSmoothShift,
        Elliptical,
        EllipticalShift,
        ClosePath,
        Error
    }
}
