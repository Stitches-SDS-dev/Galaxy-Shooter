using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class LaserMorph : MonoBehaviour
{
    private Sprite _sprite;
    private SpriteAtlas _atlas;
    private SpriteShapeParameters _shapeParams;
    private SpriteShapeRenderer _shapeRenderer;


    private void Start() {

        /*
        _sprite = GetComponent<SpriteRenderer>().sprite;
        if (_sprite == null)
            Debug.LogError("Sprite is NULL!");

        _atlas = new SpriteAtlas();

        Debug.Log(_atlas.spriteCount);*/

        
    }

    public void DrawDebug() {

        Debug.Log("Drawing Lines");

        ushort[] triangles = _sprite.triangles;
        Vector2[] vertices = _sprite.vertices;

        Debug.Log("Triangles: " + triangles.Length + ", Vertices: " + vertices.Length);

        int a, b, c;

        // draw the triangles using grabbed vertices
        for (int i = 0; i < triangles.Length; i = i + 3) {

            a = triangles[i];
            b = triangles[i + 1];
            c = triangles[i + 2];

            //To see these you must view the game in the Scene tab while in Play mode
            Debug.DrawLine(vertices[a], vertices[b], Color.green, 100.0f);
            Debug.DrawLine(vertices[b], vertices[c], Color.green, 100.0f);
            Debug.DrawLine(vertices[c], vertices[a], Color.green, 100.0f);
        }
    }

    public void ChangeSprite() {
        //Fetch the Sprite and vertices from the SpriteRenderer

        Vector2[] spriteVertices = _sprite.vertices;

        for (int i = 0; i < spriteVertices.Length; i++) {
            spriteVertices[i].x = Mathf.Clamp(
                (_sprite.vertices[i].x - _sprite.bounds.center.x -
                    (_sprite.textureRectOffset.x / _sprite.texture.width) + _sprite.bounds.extents.x) /
                (2.0f * _sprite.bounds.extents.x) * _sprite.rect.width,
                0.0f, _sprite.rect.width);

            spriteVertices[i].y = Mathf.Clamp(
                (_sprite.vertices[i].y - _sprite.bounds.center.y -
                    (_sprite.textureRectOffset.y / _sprite.texture.height) + _sprite.bounds.extents.y) /
                (2.0f * _sprite.bounds.extents.y) * _sprite.rect.height,
                0.0f, _sprite.rect.height);

            // Make a small change to the second vertex
            if (i == 2) {
                //Make sure the vertices stay inside the Sprite rectangle
                if (spriteVertices[i].x < _sprite.rect.size.x - 5) {
                    spriteVertices[i].x = spriteVertices[i].x + 5;
                }
                else spriteVertices[i].x = 0;
            }
        }
        //Override the geometry with the new vertices
        _sprite.OverrideGeometry(spriteVertices, _sprite.triangles);
    }
}
