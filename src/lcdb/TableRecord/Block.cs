using System;
using System.Collections.Generic;

namespace LitCAD.DatabaseServices
{
    public class Block : DBTableRecord, IEnumerable<Entity>
    {
        private List<Entity> _items = new List<Entity>();

        /// <summary>
        /// 克隆函数
        /// </summary>
        public override object Clone()
        {
            Block block = base.Clone() as Block;
            foreach (Entity item in _items)
            {
                Entity itemCopy = item.Clone() as Entity;
                block.AppendEntity(itemCopy);
            }

            return block;
        }

        protected override DBObject CreateInstance()
        {
            return new Block();
        }

        /// <summary>
        /// 添加图元
        /// </summary>
        public ObjectId AppendEntity(Entity entity)
        {
            if (entity.id != ObjectId.Null)
            {
                throw new System.Exception("entity is not newly created");
            }

            if (_id == ObjectId.Null)
            {
                _items.Add(entity);
                entity.SetParent(this);
            }
            else
            {
                _items.Add(entity);
                entity.SetParent(this);
                this.database.IdentifyObject(entity);
            }

            return entity.id;
        }

        /// <summary>
        /// 删除图元
        /// </summary>
        internal void RemoveEntity(Entity entity)
        {
            _items.Remove(entity);
        }

        #region IDBObjectContainer
        public IEnumerator<Entity> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
