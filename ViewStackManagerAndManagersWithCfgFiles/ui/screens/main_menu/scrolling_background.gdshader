shader_type canvas_item;
render_mode unshaded;

uniform vec2 direction = vec2(0.5, 0.5);
uniform float speed = 1.0;
uniform float scale = 1.0;

uniform sampler2D gradient : source_color;
uniform float mix_amount = 1.0;

void fragment() {
	vec4 input_color = texture(TEXTURE, UV / scale - (direction * TIME * speed));
	float greyscale_value = dot(input_color.rgb, vec3(0.299, 0.587, 0.114));
	vec3 sampled_color = texture(gradient, vec2(greyscale_value, 0.0)).rgb;
	
	COLOR.rgb = mix(input_color.rgb, sampled_color, mix_amount);
	COLOR.a = input_color.a;
}
