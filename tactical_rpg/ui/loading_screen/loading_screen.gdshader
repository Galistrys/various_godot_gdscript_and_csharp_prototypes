shader_type canvas_item;

uniform sampler2D noise_texture : repeat_enable;
uniform float time_speed = 0.1;
uniform float border = 4.0;
uniform float feather = 0.1;


float squircle(vec2 _uv, float _border, float _feather) {
	vec2 c = abs(_uv - vec2(0.5));
	float s = 0.0625 - length(pow(c, vec2(_border)));
	return smoothstep(0.0, _feather, s);
}


void fragment() {
	float mask_value = squircle(UV, border, feather);
	float noise_value = texture(noise_texture, UV + TIME * time_speed).r;
	//noise_value = exp(noise_value);
	
	float alpha = mask_value * max(mask_value, noise_value);
	
	COLOR.a *= alpha;
	COLOR.rgb = texture(TEXTURE, UV).rgb;
}