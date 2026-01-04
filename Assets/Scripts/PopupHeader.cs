using CIG;
using System.Collections;
using TMPro;
using UnityEngine;

public class PopupHeader : MonoBehaviour
{
	[SerializeField]
	private TMP_Text[] _texts;

	[SerializeField]
	private AnimationCurve _curve;

	[SerializeField]
	private float _curveScale;

	[SerializeField]
	private float _animationDuration;

	[SerializeField]
	private float _loopDelay;

	private Timing _timing;

	private Coroutine _animationRoutine;

	public void Initialize(Timing timing)
	{
		_timing = timing;
	}

	public void Play()
	{
		if (_animationRoutine != null)
		{
			StopCoroutine(_animationRoutine);
		}
		_animationRoutine = StartCoroutine(AnimationRoutine());
	}

	public void Stop()
	{
		if (_animationRoutine != null)
		{
			StopCoroutine(_animationRoutine);
			_animationRoutine = null;
		}
	}

	private IEnumerator AnimationRoutine()
	{
		float animationSpeed = 1f / _animationDuration;
		float totalAnimationDuration = 1f + animationSpeed * _loopDelay;
		float t = 1f;
		while (true)
		{
			t += _timing.GetDeltaTime(DeltaTimeType.Animation) * animationSpeed;
			t %= totalAnimationDuration;
			int i = 0;
			for (int num = _texts.Length; i < num; i++)
			{
				TMP_Text tMP_Text = _texts[i];
				tMP_Text.ForceMeshUpdate();
				TMP_TextInfo textInfo = tMP_Text.textInfo;
				int characterCount = textInfo.characterCount;
				float x = tMP_Text.bounds.min.x;
				float x2 = tMP_Text.bounds.max.x;
				for (int j = 0; j < characterCount; j++)
				{
					TMP_CharacterInfo tMP_CharacterInfo = textInfo.characterInfo[j];
					if (tMP_CharacterInfo.isVisible)
					{
						int vertexIndex = tMP_CharacterInfo.vertexIndex;
						int materialReferenceIndex = tMP_CharacterInfo.materialReferenceIndex;
						Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
						Vector3 vector = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, tMP_CharacterInfo.baseLine);
						vertices[vertexIndex] += -vector;
						vertices[vertexIndex + 1] += -vector;
						vertices[vertexIndex + 2] += -vector;
						vertices[vertexIndex + 3] += -vector;
						float num2 = 1f - (vector.x - x) / (x2 - x);
						float time = (t + num2) % totalAnimationDuration;
						float y = _curve.Evaluate(time) * _curveScale;
						Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, y, 0f), Quaternion.identity, Vector3.one);
						vertices[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex]);
						vertices[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 1]);
						vertices[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 2]);
						vertices[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 3]);
						vertices[vertexIndex] += vector;
						vertices[vertexIndex + 1] += vector;
						vertices[vertexIndex + 2] += vector;
						vertices[vertexIndex + 3] += vector;
					}
				}
				tMP_Text.UpdateVertexData();
			}
			yield return null;
		}
	}
}
