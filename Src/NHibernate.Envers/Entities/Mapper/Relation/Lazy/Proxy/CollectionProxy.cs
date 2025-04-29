using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Collection;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class CollectionProxy<T> : ICollection<T>, ILazyInitializedCollection, INotifyCollectionChanged
	{
		[NonSerialized]
		private readonly IInitializor _initializor;
		private ICollection<T> _collection;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected CollectionProxy(IInitializor initializor)
		{
			_initializor = initializor;
		}

		protected TConcrete GetCollection<TConcrete>()
		{
			if (_collection == null)
			{
				_collection = (ICollection<T>) _initializor.Initialize();
			}

			return (TConcrete) _collection;
		}
		

		public IEnumerator<T> GetEnumerator()
		{
			return GetCollection<ICollection<T>>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			GetCollection<ICollection<T>>().Add(item);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
		}

		public void Clear()
		{
			GetCollection<ICollection<T>>().Clear();
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public bool Contains(T item)
		{
			return GetCollection<ICollection<T>>().Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			GetCollection<ICollection<T>>().CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			var removeResult = GetCollection<ICollection<T>>().Remove(item);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			return removeResult;
		}

		public int Count => GetCollection<ICollection<T>>().Count;

		public bool IsReadOnly => GetCollection<ICollection<T>>().IsReadOnly;


		public Task ForceInitializationAsync(CancellationToken cancellationToken)
		{
			ForceInitialization();
			return Task.CompletedTask;
		}

		public void ForceInitialization()
		{
			GetCollection<ICollection<T>>();
		}

		public bool WasInitialized => _collection != null;
	}
}