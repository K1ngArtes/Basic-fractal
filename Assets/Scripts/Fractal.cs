using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
	public Mesh[] meshes;
	public Material material;
	public int maxDepth;
	public float childScale;
	public float spawnProbability;
	public float maxRotationSpeed;
	public float maxTwist;

	private int depth;
	private float rotationSpeed;
	// Needed to enable dynamic batching
	private Material[,] materials;
	private static Vector3[] childrenDirections = {
		Vector3.up,
		Vector3.right,
		Vector3.left,
		Vector3.forward,
		Vector3.back
	};

	private static Quaternion[] childrenOrientation = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f)
	};

	// Use this for initialization
	void Start ()
	{
		rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
		transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
		// Only root cube initializes materials
		if(materials == null)
		{
			InitializeMaterials();
		}
		gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
		gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];
		if(depth < maxDepth)
		{
			StartCoroutine(CreateChildren());
		}
	}

	private void Update()
	{
		transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
	}

	// Create children for all 4 directions
	private IEnumerator CreateChildren()
	{
		for(int i = 0; i < childrenDirections.Length; i++)
		{
			// Choose if branch will be spawned randomly
			if(Random.value < spawnProbability)
			{
				yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
				new GameObject("Fractal Child").AddComponent<Fractal>().
					Initialize(this, i);
			}
		}
	}

	// Create single cube with certain parent. For children will be called before Start
	private void Initialize(Fractal parent, int childIndex)
	{
		meshes = parent.meshes;
		materials = parent.materials;
		maxDepth = parent.maxDepth;
		childScale = parent.childScale;
		depth = parent.depth + 1;
		spawnProbability = parent.spawnProbability;
		maxRotationSpeed = parent.maxRotationSpeed;
		maxTwist = parent.maxTwist;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		transform.localPosition = childrenDirections[childIndex] * (0.5f + 0.5f * childScale);
		transform.localRotation = childrenOrientation[childIndex];
	}

	// Initialize material by assigning it a color by interpolation
	private void InitializeMaterials()
	{
		materials = new Material[maxDepth + 1, 2];

		for(int i = 0; i <= maxDepth; i++)
		{
			float t = i / (maxDepth - 1f);
			t *= t;
			materials[i, 0] = new Material(material);
			// Lerp(a, b , t) => a + (b - a) * t
			materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
			materials[i, 1] = new Material(material);
			materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
		}
		materials[maxDepth, 0].color = Color.magenta;
		materials[maxDepth, 1].color = Color.red;
	}
}
