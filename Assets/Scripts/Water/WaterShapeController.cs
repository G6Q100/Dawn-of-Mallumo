using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteAlways]
public class WaterShapeController : MonoBehaviour
{
    int cornersCount = 2;

    [SerializeField] float springForce = 0.1f,
        dampening = 0.03f, spread = 0.006f;

    [SerializeField] List<WaterSpring> springs = new();

    [SerializeField] SpriteShapeController spriteShapeController;
    [Range(1, 100)][SerializeField] int waterCount = 7, waterSize;

    [SerializeField] GameObject waterPointPrefab, waterPoints;

    private void OnValidate()
    {
        StartCoroutine(CreateWater());
    }

    IEnumerator CreateWater()
    {
        foreach (Transform child in waterPoints.transform)
        {
            StartCoroutine(DestroyWater(child.gameObject));
        }
        yield return null;
        SetWater();
        yield return null;
    }

    IEnumerator DestroyWater(GameObject oldWater)
    {
        yield return null;
        DestroyImmediate(oldWater);
    }

    void SetWater()
    {
        Spline waterSpline = spriteShapeController.spline;
        int waterPointsCount = waterSpline.GetPointCount();

        for (int i = cornersCount; i < waterPointsCount - cornersCount; i++)
            waterSpline.RemovePointAt(cornersCount);

        Vector3 waterTopLeftCorner = waterSpline.GetPosition(1);
        Vector3 waterTopRightCorner = waterSpline.GetPosition(2);
        float waterWidth = waterTopRightCorner.x - waterTopLeftCorner.x;

        float spacingPerWater = waterWidth / (waterCount + 1);

        for(int i = waterCount; i > 0; i--)
        {
            int index = cornersCount;

            float xPosition = waterTopLeftCorner.x + (spacingPerWater * i);
            Vector3 waterPoint = new Vector3(xPosition, waterTopLeftCorner.y, waterTopLeftCorner.z);
            waterSpline.InsertPointAt(index, waterPoint);
            waterSpline.SetHeight(index, 0.1f);
            waterSpline.SetCorner(index, false);

            waterSpline.SetTangentMode(index, ShapeTangentMode.Continuous);
        }

        springs = new();

        for (int i = 0; i <= waterCount + 1; i++)
        {
            int index = i + 1;

            Smoothen(waterSpline, index);

            GameObject waterPoint = Instantiate(waterPointPrefab, waterPoints.transform, false);
            waterPoint.transform.parent = transform;
            waterPoint.transform.localScale = Vector3.one * 0.001f * waterSize;
            waterPoint.transform.parent = waterPoints.transform;

            waterPoint.transform.localPosition = waterSpline.GetPosition(index);
            WaterSpring waterSprings = waterPoint.GetComponent<WaterSpring>();
            waterSprings.Init(spriteShapeController);
            springs.Add(waterSprings);
        }
    }

    void Smoothen(Spline waterSpline, int index)
    {
        Vector3 position = waterSpline.GetPosition(index);
        Vector3 positionPrev = position;
        Vector3 positionNext = position;
        if (index > 1)
        {
            positionPrev = waterSpline.GetPosition(index - 1);
        }
        if (index - 1 <= waterCount)
        {
            positionNext = waterSpline.GetPosition(index + 1);
        }

        Vector3 forward = gameObject.transform.forward;

        float scale = Mathf.Min((positionNext - position).magnitude, 
            (positionPrev - position).magnitude) * 0.33f;

        Vector3 leftTangent = (positionPrev - position).normalized * scale;
        Vector3 rightTangent = (positionNext - position).normalized * scale;
        SplineUtility.CalculateTangents(position, positionPrev, positionNext, 
            forward, scale, out rightTangent, out leftTangent);

        waterSpline.SetLeftTangent(index, leftTangent);
        waterSpline.SetRightTangent(index, rightTangent);
    }

    void UpdateSpring()
    {
        int count = springs.Count;
        float[] leftDeltas = new float[count];
        float[] rightDeltas = new float[count];
        for(int i = 0; i < count; i++)
        {
            if (i > 0)
            {
                leftDeltas[i] = spread * (springs[i].height - springs[i - 1].height);
                springs[i - 1].velocity += leftDeltas[i];
            }
            if (i < springs.Count - 1)
            {
                rightDeltas[i] = spread * (springs[i].height - springs[i + 1].height);
                springs[i + 1].velocity += rightDeltas[i];
            }
        }
    }

    void Splash(int index, float speed)
    {
        if (index >= 0 && index < springs.Count)
        {
            springs[index].velocity += speed;
        }
    }

    void FixedUpdate()
    {
        foreach(WaterSpring spring in springs)
        {
            spring.WaterSpringUpdate(springForce, dampening);
            spring.WaterPointUpdate();
        }

        UpdateSpring();
    }
}
