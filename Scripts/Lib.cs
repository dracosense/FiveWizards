using Godot;
using System;
using System.Collections.Generic;

public static class Lib
{

    public struct Vec2I
    {

        public int x, y;

        public Vec2I(int _x = 0, int _y = 0)
        {
            x = _x;
            y = _y;
        }

        public static Vec2I operator+(Vec2I a, Vec2I b)
        {
            return new Vec2I(a.x + b.x, a.y + b.y);
        }

        public static Vec2I operator-(Vec2I a, Vec2I b)
        {
            return new Vec2I(a.x - b.x, a.y - b.y);
        }

        public static bool operator==(Vec2I a, Vec2I b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator!=(Vec2I a, Vec2I b)
        {
            return !(a == b);
        }

    }

    public struct Effect
    {

        public float sTime;
        public float param;
        public int type;

        public Effect(int _type = -1, float _sTime = 0.0f, float _param = 0.0f)
        {
            type = _type;
            sTime = _sTime;
            param = _param;
        }

        public float GetPower()
        {
            switch (type)
            {
                case ATTACK_E:
                    return Mathf.Max(1.0f - ATTACK_E_CONST * param, 0.0f);
                case SPEED_E:
                    return Mathf.Max(1.0f - SPEED_E_CONST * param, 0.1f);
                case DAMAGE_E:
                    return Mathf.Max(1.0f + DAMAGE_E_CONST * param, 0.1f);
                case SHIELD_E:
                    return Mathf.Max(1.0f - SHIELD_E_CONST * param, 0.1f);
                case HEALTH_E:
                    return -HEALTH_E_CONST * param;
                case VISIBILITY_R_E:
                    return Mathf.Max(1.0f - VISIBILITY_R_E_CONST * param, 0.1f);
                default:
                    return 1.0f;
            }
        }

    }

    // wizards: nature mage, elementalist, war mage, necromancer, spirit

    public const float RAY_LENGTH = 100.0f;
    public const float ARROW_SPEED = 30.0f;
    public const float SPELL_SPEED = 20.0f;
    public const float NECROMANCER_ARROW_SPEED = 12.0f;
    public const float UNIT_ARROW_SPEED = 16.0f;
    public const float ARROW_DESTROY_TIME = 4.0f;
    public const float MIN_GO_TO_POINT_DIST = 1.0f;
    public const float BASE_TOWER_POWER = 1.8f;
    public const float TOWER_GEN_SPELL_DIST = 2.0f; //  
    public const float UNIT_RETURN_POINT_DIST = 1.0f;
    public const float UNIT_SCALE_CONST = 0.4f;
    public const float T_RAND_GEN_CONST = 0.9f;
    public const float ROOM_GEN_CONST = 0.6f;
    public const float CAMP_GEN_CONST = 0.25f;
    public const float BASIC_ARROW_DAMAGE = 0.6f;
    public const float SPELL_DAMAGE = 0.7f;
    public const float H_ARROW_DAMAGE = -1.0f;
    public const float NECROMANCER_ARROW_DAMAGE = 0.8f;
    public const float UNIT_TARGET_DIST = 1.2f;
    public const float FRIEND_UNIT_TELEPORT_DIST = 12.0f;
    public const float FRIEND_UNIT_MAX_GEN_DIST = 2.0f;
    public const float ENEMY_ATTACK_PLAYER_DIST = 1.6f;
    public const float EFFECT_TIME = 6.0f;
    public const float MAIN_P_LIGHT_BASE_R = 10.0f;
    public const float FRIEND_HEALTH_E = -2.0f;
    public const float FRIEND_H_CONST = 1.0f;
    public const float FRIEND_D_CONST = 1.0f;
    public const float FRIEND_SPEED = 7.0f;
    public const float MAP_CELL_SIZE = 4.0f;
    public const float MAX_MAGIC_E = 10.0f;
    public const float UNIT_M_E_ADD = 1.0f;
    public const float SKELETON_GEN_CONST = 0.8f;
    public const float SPIRIT_TIME_COST = 0.01f;
    public const float ELEMENTAL_MAGIC_E = 3.0f; // magic energy to gen elemental (blue)
    public const float SPIRIT_MAGIC_E = 0.8f;
    public const float R_PLAYER_M_E = 5.0f; // regenerate player magic energy
    public const float R_PLAYER_HEALTH = 30.0f;
    public const float ENT_ADD_HEALTH = 4.0f;
    public const float BLUE_W_TOWER_SPEED = 8.0f;
    public const float BLUE_W_TOWER_DAMAGE = 2.0f;
    public const float WIZARD_T_GEN_CONST = 0.8f;
    public const float ENEMY_V_RANGE = 12.0f;
    public const float ENEMY_ARCH_DIST = 6.0f;
    // effect constants
    public const float ATTACK_E_CONST = 0.6f;
    public const float SPEED_E_CONST = 0.4f;
    public const float DAMAGE_E_CONST = 0.6f;
    public const float SHIELD_E_CONST = 0.6f;
    public const float HEALTH_E_CONST = 0.3f;
    public const float VISIBILITY_R_E_CONST = 0.4f;
    //
    public const int EMPTY_TILE = -1;
    public const int BLOCK_TILE = 0;
    public const int FLOOR_TILE = 1;
    public const int TELEPORT_TILE = 2;
    public const int TOWER_GEN_CONST = 3;
    public const int TOWER_TYPES_NUM = 2; // 
    public const int EFFECTS_NUM  = 6; // 0 - monster, 1-5 - wizards
    public const int WIZARDS_NUM = 6;
    public const int MAP_TOWER_FLOORS_NUM = 2;
    public const int MAX_GEN_RAND_R_POS_NUM = 100;
    public const int MINIMAP_SIZE = 16;
    public const int DRAW_MAP_DIST = 16;
    public const int MAX_SPELL_COPIES_NUM = 3;
    public const int MAX_SPELL_NUM = 20;
    public const int MAP_FLOOR_DIST = 40;
    public const int UNIT_TYPES_NUM = 6;
    public const int RANDOM_SHUFFLE_CONST = 5;
    public const int BASE_FRIENDS_NUM = 3;
    public const int ENT_TYPE = 1;
    public const int MAIN_R_T_GEN_DIST = 2;
    public const int MAP_M_BIT = 0;
    public const int PLAYER_M_BIT = 1;
    public const int ENEMY_M_BIT = 2;
    public const int FRIEND_M_BIT = 5;
    // wizards
    public const int MONSTER_WIZARD = 0;
    public const int NATURE_WIZARD = 1;
    public const int ELEMENTAL_WIZARD = 2;
    public const int WAR_WIZARD = 3;
    public const int NECROMANCER_WIZARD = 4;
    public const int SPIRIT_WIZARD = 5;
    // structures
    public const int NULL_STRUCT = -1;
    public const int CAMP_STRUCT = 0;
    public const int TOWER_STRUCT = 1;
    // effect types
    public const int ATTACK_E = 0; 
    public const int SPEED_E = 1; 
    public const int  DAMAGE_E = 2; 
    public const int SHIELD_E = 3; 
    public const int HEALTH_E = 4;
    public const int VISIBILITY_R_E = 5;
    // unit modes
    public const int FOLLOW_PLAYER_M = 0;
    public const int DEFEND_POINT_M = 1;
    public const int ATTACK_M = 2;
    // tower types
    public const int ALL_T_TYPE = 0;
    public const int SIDE_T_TYPE = 1;
    public const int FRONT_T_TYPE = 2;
    //

    public const int D_NUM = 4; // directions
    public const int INF = (int)1e9;

    public static readonly PackedScene arrowPS = LoadPS("arrow");
    public static readonly PackedScene eArrowPS = LoadPS("e_arrow");
    public static readonly PackedScene healEArrowPS = LoadPS("heal_e_arrow");
    public static readonly PackedScene necromancerEArrowPS = LoadPS("necromancer_e_arrow");
    public static readonly PackedScene elementalArrowPS = LoadPS("elemental_arrow");
    public static readonly PackedScene spellPS = LoadPS("spell");
    public static readonly PackedScene towerPS = LoadPS("tower");
    public static readonly PackedScene campPS = LoadPS("camp");
    public static readonly PackedScene enemyUnitPS = LoadPS("enemy_unit");
    public static readonly PackedScene friendUnitPS = LoadPS("friend_unit");
    public static readonly Material damagedUnitM = LoadM("damaged_unit_m");
    public static readonly Material unitM = LoadM("main_m");
    public static readonly Vec2I MAIN_MAP_SIZE = new Vec2I(120, 120);
    public static readonly Vec2I MAP_T_FLOOR_SIZE = new Vec2I(60, 60);
    public static readonly Vec2I CAMP_UNITS_NUM = new Vec2I(2, 4);
    public static readonly Vec2I ROOM_MONSTERS_NUM = new Vec2I(4, 6);
    public static readonly Vector3 SHOOT_BASE_TRANSLATION = new Vector3(0.0f, 0.25f * MAP_CELL_SIZE, 0.0f);

    public static readonly Vec2I[] D_WAYS = {new Vec2I(1, 0), new Vec2I(0, 1), new Vec2I(-1, 0), new Vec2I(0, -1)}; // directions ways

    public static readonly PackedScene[] wizardTowers = {LoadPS("WizardTowers/red_tower"),
     LoadPS("WizardTowers/green_tower"), LoadPS("WizardTowers/blue_tower"), LoadPS("WizardTowers/gray_tower"),
      LoadPS("WizardTowers/purpure_tower"), LoadPS("WizardTowers/white_tower")};
      public static readonly PackedScene[] unitArrow = {null, null, elementalArrowPS, null, null, null};
    public static readonly Mesh[] towerTModels = {LoadMesh("tower/tower0"), 
    LoadMesh("tower/tower1"), LoadMesh("tower/tower2")};
    public static readonly Material[] towerEMaterials = {LoadM("crystal_red_m"), LoadM("crystal_green_m"),
     LoadM("crystal_blue_m"), LoadM("crystal_orange_m"), LoadM("crystal_purpure_m"), LoadM("crystal_white_m")};
     public static readonly Material[] eMaterials = {LoadM("e_red_m"), LoadM("e_green_m"), 
     LoadM("e_blue_m"), LoadM("e_orange_m"), LoadM("e_purpure_m"), LoadM("e_white_m")};
     //public static readonly Material[] wizardPMaterials = {LoadM("p_red"), LoadM("p_green"),
      //LoadM("p_blue"), LoadM("p_orange"), LoadM("p_purpure"), LoadM("p_white")}
     public static readonly string[] unitName = {"red", "green", "blue", "gray", "purpure", "white"};
    public static readonly string[] wizardName = {"blue_wizard", "blue_wizard", "blue_wizard", "blue_wizard", "blue_wizard", "blue_wizard"}; //
     public static readonly float[] unitSpeed = {4.0f, 4.0f, 4.0f, 4.0f, 4.0f, 4.0f};
     public static readonly float[] unitShield = {0.3f, 0.3f, 0.3f, 0.3f, 0.2f, 0.4f};
     public static readonly float[] unitDamage = {1.2f, 0.6f, 0.8f, 0.7f, 0.6f, 0.8f};
     public static readonly float[] unitHealth = {5.0f, 7.0f, 6.0f, 3.0f, 4.0f, 6.0f};
     public static readonly uint[,] wizardUnits = {{0}, {1}, {2}, {3}, {4}, {5}};
     public static readonly float[] wizardGenEConst = {1.2f, 0.8f, 1.0f, 1.8f, 2.0f, 1.0f};

    public static PackedScene LoadPS(string name)
    {
        try
        {
            return (PackedScene)ResourceLoader.Load("res://Scenes/" + name + ".tscn");
        }
        catch
        {
            GD.Print("Can't load " + name + '.');
            return null;
        }
    }

    public static Material LoadM(string name)
    {
        try
        {
            return (Material)ResourceLoader.Load("res://Materials/" + name + ".tres");
        }
        catch
        {
            GD.Print("Can't load " + name + '.');
            return null;
        }
    }

    public static Mesh LoadMesh(string name)
    {
        try
        {
            return (Mesh)ResourceLoader.Load("res://Models/" + name + ".obj");
        }
        catch
        {
            GD.Print("Can't load " + name + '.');
            return null;
        }
    }

    public static List<int> GenRandInRange(Vec2I range, int num, Random rand)
    {
        List<int> ans;
        int x = 0, y = 0;
        bool b = false;
        if(range.y - range.x < num || num <= 0 || rand == null)
        {
            return null;
        }
        ans = new List<int>();
        for (int i = 0; i < num; i++)
        {
            x = range.x + rand.Next() % (range.y - range.x - i);
            b = true;
            for (int j = 0; j < ans.Count; j++)
            {
                if (x < ans[j])
                {
                    y = ans[j];
                    ans[j]  = x;
                    x = y;
                    b = false;
                }
                else
                {
                    if (b)
                    {
                        x++;
                    }
                }
            }
            ans.Add(x);
        }
        return ans;
    }

    public static Vector2 Vec3ToVec2(Vector3 v)
    {
        return  new Vector2(v.x, v.z);
    }

    public static Vec2I Vec3ToI(Vector3 v)
    {
        return new Vec2I((int)v.x, (int)v.z);
    }

    public static float AngleByVec3(Vector3 v)
    {
        return (new Vector2(v.x, v.z)).Angle();
    }

    public static float GetRealAngle(float angle)
    {
        return -angle + Mathf.Pi / 2.0f;
    }

}
