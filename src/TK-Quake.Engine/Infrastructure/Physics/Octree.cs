using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Physics
{
    public class Octree
    {
        public int MaxDepth { get; set; }
        public int MinItemsPerTree { get; set; }
        public int MaxItemsPerTree { get; set; }

        public Vector3 Corner1 { get; set; }
        public Vector3 Corner2 { get; set; }

        private Vector3 _center;
        public Vector3 Center
        {
            get { return _center; }
        }

        private bool _hasChildren;
        public bool HasChildren { get { return _hasChildren; } }

        public int Depth { get; private set; }
        public int ItemCount { get; private set; }

        private List<Entity> _collection = new List<Entity>();
        private Octree[, ,] _children = new Octree[2, 2, 2]; // A multidimensional cube of Octrees


        //Constructs a new Octree. corner1 is (minX, minY, minZ), corner2 is (maxX, maxY, maxZ)
        public Octree(Vector3 corner1, Vector3 corner2, int depth = 1)
        {
            this.Corner1 = corner1;
            this.Corner2 = corner2;
            this.Depth = depth;
            ItemCount = 0;
            _hasChildren = false;

            MinItemsPerTree = 2;
            MaxItemsPerTree = 2;
            MaxDepth = 3;

            // Temp until Vector division is added
            Vector3 sum = Corner1 + Corner2;
            _center = sum / 2;

        }

        public void Add(ref Entity item)
        {
            ItemCount++;

            if (!_hasChildren && Depth < MaxDepth && ItemCount > MaxItemsPerTree)
                MakeChildren();

            if (_hasChildren)
                File(item, item.Position, true);
            else
                _collection.Add(item);
        }

        //Removes an item from the Octree
        public void Remove(ref Entity item)
        {
            Remove(item, item.Position);
        }


        //Changes the position of a ball in this from oldPos to ball->pos
        public void UpdateTree()
        {

            // Recurse down and update children nodes
            if (_hasChildren)
            {
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        for (int z = 0; z < 2; z++)
                        {
                            _children[x, y, z].UpdateTree();
                        }
                    }
                }
            }


            // This will hit a runtime error. Need to use Iterators
            List<Entity> items = _collection.ToList();
            _collection.ForEach(i => Remove(i, i.Position));
            items.ForEach(i => Add(ref i));
        }

        //Adds potential ball-ball collisions to the specified set
        //    public void potentialBallBallCollisions(vector<BallPair> &collisions)
        //    {
        //        if (hasChildren)
        //        {
        //            for(int x = 0; x < 2; x++)
        //            {
        //                for(int y = 0; y < 2; y++) {
        //                    for(int z = 0; z < 2; z++) {
        //                        children[x][y][z]->
        //                            potentialBallBallCollisions(collisions);
        //                    }
        //                }
        //            }
        //        }
        //        else {
        //            //Add all pairs (ball1, ball2) from balls
        //            for(set<Ball*>::iterator it = balls.begin(); it != balls.end();
        //                    it++) {
        //                Ball* ball1 = *it;
        //                for(set<Ball*>::iterator it2 = balls.begin();
        //                        it2 != balls.end(); it2++) {
        //                    Ball* ball2 = *it2;
        //                    //This test makes sure that we only add each pair once
        //                    if (ball1 < ball2) {
        //                        BallPair bp;
        //                        bp.ball1 = ball1;
        //                        bp.ball2 = ball2;
        //                        collisions.push_back(bp);
        //                    }
        //                }
        //            }
        //        }
        //}



        // Private Methods

        //Adds an item to or removes one from the children of this
        private void File(Entity item, Vector3 position, bool add)
        {
            //Figure out in which child(ren) the item belongs

            // Get center for quick reference
            Vector3 center = this.Center;

            for (int x = 0; x < 2; x++)
            {
                if (x == 0)
                {
                    if (position.X - item.Position.X > center.X)
                    {
                        continue;
                    }
                }
                else if (position.X + item.Position.X < center.X)
                {
                    continue;
                }

                for (int y = 0; y < 2; y++)
                {
                    if (y == 0)
                    {
                        if (position.Y - item.Position.Y > center.Y)
                        {
                            continue;
                        }
                    }
                    else if (position.Y + item.Position.Y < center.Y)
                    {
                        continue;
                    }

                    for (int z = 0; z < 2; z++)
                    {
                        if (z == 0)
                        {
                            if (position.Z - item.Position.Z > center.Z)
                            {
                                continue;
                            }
                        }
                        else if (position.Z + item.Position.Z < center.Z)
                        {
                            continue;
                        }

                        //Add or remove the item
                        if (add)
                        {
                            _children[x, y, z].Add(ref item);
                        }
                        else
                        {
                            _children[x, y, z].Remove(item, position);
                        }
                    }
                }
            }
        }

        //Creates children of this, and moves the items in this to the _children
        private void MakeChildren()
        {
            for (int x = 0; x < 2; x++)
            {
                float minX;
                float maxX;
                if (x == 0)
                {
                    minX = Corner1.X;
                    maxX = this.Center.X;
                }
                else
                {
                    minX = this.Center.X;
                    maxX = Corner2.X;
                }

                for (int y = 0; y < 2; y++)
                {
                    float minY;
                    float maxY;
                    if (y == 0)
                    {
                        minY = Corner1.Y;
                        maxY = this.Center.Y;
                    }
                    else
                    {
                        minY = this.Center.Y;
                        maxY = Corner2.Y;
                    }

                    for (int z = 0; z < 2; z++)
                    {
                        float minZ;
                        float maxZ;
                        if (z == 0)
                        {
                            minZ = Corner1.Z;
                            maxZ = this.Center.Z;
                        }
                        else
                        {
                            minZ = this.Center.Z;
                            maxZ = Corner2.Z;
                        }

                        _children[x, y, z] = new Octree(
                            new Vector3(minX, minY, minZ),
                            new Vector3(maxX, maxY, maxZ),
                            Depth + 1);
                    }
                }
            }

            foreach (var item in _collection)
            {
                File(item, item.Position, true);
            }
            _collection.Clear();

            _hasChildren = true;
        }

        //Adds all items in this or one of its descendants to the specified set
        private void CollectChildren(List<Entity> items)
        {
            if (_hasChildren)
            {
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        for (int z = 0; z < 2; z++)
                        {
                            _children[x, y, z].CollectChildren(items);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in _collection)
                {
                    items.Add(item);
                }
            }
        }

        //Destroys the children of this, and moves all items in its descendants
        //to this octree's _collection set
        private void DestroyChildren()
        {
            //Move all items in descendants of this to the _collection set
            CollectChildren(_collection);

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < 2; z++)
                    {
                        _children[x, y, z] = null;
                    }
                }
            }

            _hasChildren = false;
        }

        //Removes the specified item at the indicated position
        private void Remove(Entity item, Vector3 position)
        {
            ItemCount--;

            if (_hasChildren && ItemCount < MinItemsPerTree)
            {
                DestroyChildren();
            }

            if (_hasChildren)
            {
                File(item, position, false); // Remove
            }
            else
            {
                _collection.Remove(item);
            }
        }

    }
}
