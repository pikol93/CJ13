shader_type spatial;
// Prevent the quad from being affected by lighting and fog. This also improves performance.
render_mode unshaded, fog_disabled;

uniform float levels: hint_range(1.0, 32.0) = 16.0;

uniform sampler2D screen_texture: hint_screen_texture, filter_linear_mipmap;
uniform sampler2D noise;

void vertex() {
  POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {
	vec3 noise_current = (texture(noise, SCREEN_UV).rgb - vec3(0.5)) / 1000.0;
	vec3 current = texture(screen_texture, SCREEN_UV).rgb + noise_current;
	float greyscale = max(current.r, max(current.g, current.b));
	float lower = floor(greyscale * levels) / levels;
	float lowerDifference = abs(greyscale - lower);
	float upper = ceil(greyscale * levels) / levels;
	float upperDifference = abs(upper - greyscale);
	float level = lowerDifference <= upperDifference ? lower : upper;
	float adjustment = level / greyscale;
	
	ALBEDO = current * adjustment;
}
