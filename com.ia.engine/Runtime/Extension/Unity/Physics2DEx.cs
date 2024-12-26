using UnityEngine;

namespace IAEngine
{
    public class Physics2DEx
    {
        private static Color DefaultColor = new Color(0,0,1,1);

        public static int BoxCastNonAllocDraw(Vector2 origin, Vector2 size, Vector2 direction, RaycastHit2D[] results,
            float distance, int layer_mask, Color noColor, Color hitColor)
        {
            float angle = 0f;
            int rchCnt = Physics2D.BoxCastNonAlloc(origin, size, angle, direction, results, distance, layer_mask);

            float half_width = size.x / 2;
            float half_height = size.y / 2;
 
            // 4 points of the origin box
            Vector2 p1 = new Vector2(origin.x - half_width, origin.y + half_height);
            Vector2 p2 = new Vector2(origin.x + half_width, origin.y + half_height);
            Vector2 p3 = new Vector2(origin.x - half_width, origin.y - half_height);
            Vector2 p4 = new Vector2(origin.x + half_width, origin.y - half_height);
 
            // 4 points of the destination box
            Vector2 dest_origin = origin + (distance * direction);
            Vector2 t1 = new Vector2(dest_origin.x - half_width, dest_origin.y + half_height);
            Vector2 t2 = new Vector2(dest_origin.x + half_width, dest_origin.y + half_height);
            Vector2 t3 = new Vector2(dest_origin.x - half_width, dest_origin.y - half_height);
            Vector2 t4 = new Vector2(dest_origin.x + half_width, dest_origin.y - half_height);
 
            Color box_color = rchCnt > 0 ? hitColor : noColor; // If it's a hit, turn GREEN, else color is BLUE
 
            Debug.DrawLine(p1, p2, box_color);
            Debug.DrawLine(p2, p4, box_color);
            Debug.DrawLine(p4, p3, box_color);
            Debug.DrawLine(p3, p1, box_color);
 
            Debug.DrawLine(t1, t2, box_color);
            Debug.DrawLine(t2, t4, box_color);
            Debug.DrawLine(t4, t3, box_color);
            Debug.DrawLine(t3, t1, box_color);
 
            Debug.DrawLine(p1, t1, box_color);
            Debug.DrawLine(p2, t2, box_color);
            Debug.DrawLine(p3, t3, box_color);
            Debug.DrawLine(p4, t4, box_color);
            
            return rchCnt;
        }
        
        public static int BoxCastNonAllocDraw(Vector2 origin, Vector2 size, Vector2 direction, RaycastHit2D[] results, float distance, int layer_mask)
        {
            return BoxCastNonAllocDraw(origin, size, direction, results, distance, layer_mask, Color.blue, Color.green);
        }

        public static bool OverlapBoxDraw(Vector2 pPos, Vector2 pSize, int layerMask)
        {
            bool res = Physics2D.OverlapBox(pPos, pSize, 0, layerMask);

            Color box_color = res ? Color.green : Color.blue;
            
            float half_width = pSize.x / 2;
            float half_height = pSize.y / 2;

            Vector2 p1 = new Vector2(pPos.x - half_width, pPos.y + half_height);
            Vector2 p2 = new Vector2(pPos.x + half_width, pPos.y + half_height);
            Vector2 p3 = new Vector2(pPos.x - half_width, pPos.y - half_height);
            Vector2 p4 = new Vector2(pPos.x + half_width, pPos.y - half_height);
            
            Debug.DrawLine(p1, p2, box_color);
            Debug.DrawLine(p2, p4, box_color);
            Debug.DrawLine(p4, p3, box_color);
            Debug.DrawLine(p3, p1, box_color);

            return res;
        }
        
        public static int OverlapPointDraw(Vector2 pPos, Collider2D[] resCols, int layerMask)
        {
            int resCnt = Physics2D.OverlapPointNonAlloc(pPos, resCols, layerMask);
            
            Debug.LogWarning("pPos>>"+pPos);

            Color box_color = resCnt > 0 ? Color.green : Color.blue;
            
            float half_width = 0.2f / 2;
            float half_height = 0.2f / 2;

            Vector2 p1 = new Vector2(pPos.x - half_width, pPos.y + half_height);
            Vector2 p2 = new Vector2(pPos.x + half_width, pPos.y + half_height);
            Vector2 p3 = new Vector2(pPos.x - half_width, pPos.y - half_height);
            Vector2 p4 = new Vector2(pPos.x + half_width, pPos.y - half_height);
            
            Debug.DrawLine(p1, p2, box_color);
            Debug.DrawLine(p2, p4, box_color);
            Debug.DrawLine(p4, p3, box_color);
            Debug.DrawLine(p3, p1, box_color);

            return resCnt;
        }
        
    }
}