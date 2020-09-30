namespace SchedulerConst
{

    /**
* date 2018.07.12
* author Lugub
* desc
*  객체의 고유번호, Item.cs의 OriginNumber가 이 테이블에 있는 거임
*  예시로 사용하는 객체 큐브, 스피어, 실린더는 1,2,3으로 사용해 보고
*  실제 사용은 1000부터
*/
    public static class ID
    {
        public static class ObjectID
        {
            public const int Cube       = 1;
            public const int Sphere     = 2;
            public const int Cylinder   = 3;
            






        }

        public static class PlaceID
        {

        }

    }


    public static class Const
    {
        // item
        public const int ITEM_SCALE_X = 1;
        public const int ITEM_SCALE_Y = 1;
        public const int ITEM_SCALE_Z = 1;
        public const float ANI_BAR_DEFALUT_POS_X = 178.8f;
        public const int LEFT_POSITION = -1;
        public const int RIGHT_POSITION = 1;
        public const float ANI_BAR_MIN_WIDTH_SIZE = 20;
        public const float ANI_BAR_DEFALUT_HEIGHT = 50;

        // time and cursur
        public const long DEFAULT_TIME = 0;
        public const long MAX_TIME = 192000;
        public const int LISTVIEW_WIDTH = 1680;
        public const int LISTVIEW_TIMEBAR_WIDTH = 1380;
    }

    public class CoordinateVar
    {
        public float CURSOR_POS_X_DEFAULT = 178.8f; // 커서의 최초 위치, 왼쪽의 한계선 좌표
        public float CURSOR_POS_X_MAX = 998.7f;     // 커서의 오른쪽 한계선 좌표
        public float CURSOR_MOVE_RATE = 0.07f;      // 1 millisecond가 흐를 때 움직이는 거리
        public float CURSOR_TIME_OF_MOVE = 234.5f;  // 좌표 1의 이동에 따른 시간 변화
        public float ITEM_MIN_WIDTH_SIZE = 20;
        public float TIME_BAR_WITH = 1680;  // 유니티 내의 비율과 현재 해상도 상에서의 비율을 통해 timebar의 길이 구하기
        public float COORDINATE_OF_ONE_WIDTH = 1; // 1 width당 좌표계 길이
    }

}