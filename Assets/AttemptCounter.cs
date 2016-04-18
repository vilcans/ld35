using UnityEngine;

public class AttemptCounter : MonoBehaviour {
	private static int count = 0;

	private TextMesh textMesh;
	private Material material;

	private float scaleProgress = 0;
	private float scaleTime = 2;

	private Vector3 startScale;
	private Vector3 endScale;

	public void Start() {
		++count;
		textMesh = GetComponent<TextMesh>();
		textMesh.text = "Attempt " + count;

		startScale = textMesh.transform.localScale;
		endScale = textMesh.transform.localScale * 2.0f;

		material = GetComponent<MeshRenderer>().material;
	}

	public void Update() {
		scaleProgress += Time.deltaTime;
		float t = scaleProgress / scaleTime;
		textMesh.transform.localScale = Vector3.Lerp(startScale, endScale, t);
		Color color = material.color;
		color.a = Mathf.Clamp01(1.0f - t * t);
		material.color = color;
	}
}
