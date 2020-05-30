using Godot;
using System;
using static Lib;
using static GameMap;

public class Minimap : TileMap
{

    private Root root;
    private MainMap map;
    private Vec2I pos;
    private Vec2I lastPos;
    public bool redrawMap;

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        map = (MainMap)GetNode("/root/Game/MainMap");
        redrawMap = true;
    }

    public override void _Process(float delta)
    {
        MapCell cell;
        Vector3 u;
        Vector2 v;
        int x = 0;
        int y = 0;
        u = root.playerPos;
        u.x /= map.CellSize.x;
        u.y /= map.CellSize.y;
        u.z /= map.CellSize.z;
        v = -Vec3ToVec2(u) + MINIMAP_SIZE * (new Vector2(1.0f, 1.0f));
        v.x *= this.CellSize.x * this.Scale.x;
        v.y *= this.CellSize.y * this.Scale.y;
        this.Position = v;
        lastPos = pos;
        pos = root.pMapPos;
        if (pos != lastPos || redrawMap)
        {
            for (int i = lastPos.x - MINIMAP_SIZE; i <= lastPos.x + MINIMAP_SIZE; i++)
            {
                for (int j = lastPos.y - MINIMAP_SIZE; j <= lastPos.y + MINIMAP_SIZE; j++)
                {
                    if (Mathf.Abs(i - pos.x) > MINIMAP_SIZE || Mathf.Abs(j - pos.y) > MINIMAP_SIZE)
                    {
                        SetCell(i, j, -1);
                    }
                }
            }
            for (int i = pos.x - MINIMAP_SIZE; i <= pos.x + MINIMAP_SIZE; i++)
            {
                for (int j = pos.y - MINIMAP_SIZE; j <= pos.y + MINIMAP_SIZE; j++)
                {
                    if (Mathf.Abs(i - lastPos.x) > MINIMAP_SIZE || Mathf.Abs(j - lastPos.y) > MINIMAP_SIZE || redrawMap)
                    {
                        cell = map.GetMapCell(new Vec2I(i, j));
                        if (cell == null)
                        {
                            x = -1;
                        }
                        else
                        {
                            switch(cell.tile)
                            {
                                case BLOCK_TILE:
                                    x = 0;
                                    break;
                                case FLOOR_TILE:
                                    if (cell.tower_t == -1)
                                    {
                                        y = map.GetRoomOwner(cell.room);
                                        if (map.GetRoomGenerated(cell.room) || y == root.playerWizard || y < 0)
                                        {
                                            x = 1;
                                        }
                                        else
                                        {
                                            x = 4;
                                        }
                                    }
                                    else
                                    {
                                        x = 2;
                                    }
                                    break;
                                case TELEPORT_TILE:
                                    x = 3;
                                    break;
                                default:
                                    x = -1;
                                    break;
                            }
                        }
                        SetCell(i, j, x);
                    }
                }
            }
        }
        redrawMap = false;
    }

}
