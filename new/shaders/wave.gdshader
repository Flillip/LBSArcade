shader_type canvas_item;

uniform float progress: hint_range(0.0, 1.0) = 0.5;
uniform vec4 bg_color: source_color;
uniform vec4 wave_1_color: source_color;
uniform vec4 wave_2_color: source_color;
uniform float wave_1_speed = -1.0;
uniform float wave_2_speed = 1.0;
uniform float wave_1_freq = 2.5;
uniform float wave_2_freq = 2.0;
uniform float wave_1_amp = 0.13;
uniform float wave_2_amp = 0.1;
uniform float wave_1_sin_shift_horizontal = 0.0;
uniform float wave_2_sin_shift_horizontal = 0.7785;

vec4 wave( vec2 uv, vec4 wave_color, float level, float freq, float amp, float sin_shift, float speed ){
	float sinus = sin( (uv.x - sin_shift + TIME * speed) * freq) * amp;
	float shifted_level = ( 1.0 + 2.0 * amp ) * level - amp; //shift to completely hide or fill
	float treshold = step( 1.0 - sinus - shifted_level, uv.y);
	return wave_color * treshold;
}

void fragment() {
	
	vec4 wave1 = wave(UV, wave_1_color, progress, wave_1_freq, wave_1_amp, wave_1_sin_shift_horizontal, wave_1_speed);
	vec4 wave2 = wave(UV, wave_2_color, progress, wave_2_freq, wave_2_amp, wave_2_sin_shift_horizontal, wave_2_speed);
	
	vec4 combined_waves = mix(wave1, wave2, wave2.a);
	COLOR.rgb = mix( bg_color, combined_waves, combined_waves.a ).rgb;
}
