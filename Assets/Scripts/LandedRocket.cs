using UnityEngine;
using UnityEngine.Rendering;

public class LandedRocket : MonoBehaviour
{
    private const float MoonPixelsPerUnit = 32f;
    private const float RocketScale = 2.5f;
    private const int RocketSortingOrder = 2;
    private static readonly Vector3 PlayerStepOffPad = new Vector3(0f, -1f, 0f);
    private static readonly Vector2 BaseColliderSize = new Vector2(1.1f, 0.45f);
    private static readonly Vector2 BaseColliderOffset = new Vector2(0f, 0.2f);

    private void Start()
    {
        if (!RocketBuild.HasAnyPart)
        {
            return;
        }

        GameObject player = GameObject.Find("Player");
        Vector3 padCenter = player != null ? player.transform.position : Vector3.zero;
        if (player != null)
        {
            player.transform.position = padCenter + PlayerStepOffPad;
        }

        GameObject rocket = new GameObject("Player Rocket");
        rocket.transform.position = padCenter;
        rocket.AddComponent<SortingGroup>().sortingOrder = RocketSortingOrder;

        BoxCollider2D baseCollider = rocket.AddComponent<BoxCollider2D>();
        baseCollider.size = BaseColliderSize;
        baseCollider.offset = BaseColliderOffset;

        Sprite[] partsFromBottom = { RocketBuild.ThrusterSprite, RocketBuild.BodySprite, RocketBuild.CabinSprite };
        float stackTop = 0f;
        int partOrder = 0;
        foreach (Sprite part in partsFromBottom)
        {
            if (part == null)
            {
                continue;
            }
            stackTop = StackPart(rocket.transform, part, stackTop, partOrder);
            partOrder++;
        }
    }

    private static float StackPart(Transform rocket, Sprite sprite, float stackTop, int sortingOrder)
    {
        float scale = sprite.pixelsPerUnit / MoonPixelsPerUnit * RocketScale;
        float onePixel = RocketScale / MoonPixelsPerUnit;

        GameObject partObject = new GameObject(sprite.name);
        partObject.transform.SetParent(rocket, false);
        partObject.transform.localScale = new Vector3(scale, scale, 1f);

        SpriteRenderer renderer = partObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = sortingOrder;

        Bounds bounds = sprite.bounds;
        partObject.transform.localPosition = new Vector3(-bounds.center.x * scale, stackTop - bounds.min.y * scale, 0f);
        return stackTop + bounds.size.y * scale - onePixel;
    }
}
