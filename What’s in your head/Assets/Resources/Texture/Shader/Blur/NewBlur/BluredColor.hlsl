void BluredColor_float(float4 Seed, float Min, float Max, float BlurX, float BlurY, out float4 Out)
{
	// 시드를 통해 랜덤 난수를 생성
	float randomno = frac(sin(dot(Seed.xy, float2(12.9898, 78.233))) * 43758.5453);

	// 랜덤 난수를 통해 최소 최대값을 보간처리
	float noise = lerp(Min, Max, randomno);

	// x와 y값을 변형해서 블러처리를 만들어줌
	float uvx = float(sin(noise)) * BlurX;
	float uvy = float(cos(noise)) * BlurY;

	// 완성된 값을 내보냄.
	float4 uvpos = float4(Seed.x + uvx, Seed.y + uvy, Seed.zw);
	Out = uvpos;
}


