shader_type canvas_item;

uniform float bob_speed = 3.5;
uniform float bob_height = 0.25;
uniform float bob_offset = 0.0;

void vertex() {
	float seed = float(INSTANCE_ID) * 1.37;

	VERTEX.y += sin((TIME+seed) * bob_speed + bob_offset) * bob_height - bob_height;
}
