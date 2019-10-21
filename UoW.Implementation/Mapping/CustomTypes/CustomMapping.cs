using FluentNHibernate.Mapping;
using SimplePersistence.Model;
using UoW.Implementation.Mapping.CustomTypes.Types;

namespace UoW.Implementation.Mapping.CustomTypes
{
	public static class CustomMapping
	{
		/// <summary>
		/// Maps all properties of an entity with <see cref="IHaveSoftDelete"/>.
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="classMap"></param>
		/// <returns></returns>
		public static ClassMap<TModel> MapSifarmaSoftDelete<TModel>(this ClassMap<TModel> classMap)
			where TModel : IHaveSoftDelete
		{
			classMap.Map(x => x.Deleted, "APAGADO").CustomType<YesNoCharBoolean>().Not.Nullable();

			return classMap;
		}
		/// <summary>
		/// Maps all properties of an <see cref="IHaveCreatedMeta"/> model
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="classMap"></param>
		/// <param name="nullable"></param>
		public static ClassMap<TModel> MapCreatedMeta<TModel>(this ClassMap<TModel> classMap, bool nullable = false)
			where TModel : IHaveCreatedMeta
		{
			var createdOnMap = classMap.Map(x => x.CreatedOn, "CREATED_ON").CustomType<NormalizedDateTimeUserType>();
			var createdByMap = classMap.Map(x => x.CreatedBy, "CREATED_BY").Length(64);

			if (nullable)
			{
				createdOnMap.Nullable();
				createdByMap.Nullable();
			}
			else
			{
				createdOnMap.Not.Nullable();
				createdByMap.Not.Nullable();
			}

			return classMap;
		}

		/// <summary>
		/// Maps all properties of an <see cref="IHaveCreatedMeta"/> model using TimestampWTimeZoneUserType
		/// for DateTimeOffset properties
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="classMap"></param>
		/// <param name="nullable"></param>
		public static ClassMap<TModel> MapCreatedMetaTimestamp<TModel>(this ClassMap<TModel> classMap, bool nullable = false)
			where TModel : IHaveCreatedMeta
		{
			var createdOnMap = classMap.Map(x => x.CreatedOn, "CREATED_ON").CustomType<TimestampUserType>();
			var createdByMap = classMap.Map(x => x.CreatedBy, "CREATED_BY").Length(64);

			if (nullable)
			{
				createdOnMap.Nullable();
				createdByMap.Nullable();
			}
			else
			{
				createdOnMap.Not.Nullable();
				createdByMap.Not.Nullable();
			}

			return classMap;
		}


		/// <summary>
		/// Maps all properties of an <see cref="IHaveUpdatedMeta"/> model
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="classMap"></param>
		/// <param name="nullable"></param>
		/// <returns></returns>
		public static ClassMap<TModel> MapUpdatedMeta<TModel>(this ClassMap<TModel> classMap, bool nullable = false)
			where TModel : IHaveUpdatedMeta
		{
			var updatedOnMap = classMap.Map(x => x.UpdatedOn, "UPDATED_ON").CustomType<NormalizedDateTimeUserType>();
			var updatedByMap = classMap.Map(x => x.UpdatedBy, "UPDATED_BY").Length(64);

			if (nullable)
			{
				updatedOnMap.Nullable();
				updatedByMap.Nullable();
			}
			else
			{
				updatedOnMap.Not.Nullable();
				updatedByMap.Not.Nullable();
			}

			return classMap;
		}


		/// <summary>
		/// Maps all properties of an <see cref="IHaveUpdatedMeta"/> model using TimestampWTimeZoneUserType
		/// for DateTimeOffset properties
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="classMap"></param>
		/// <param name="nullable"></param>
		/// <returns></returns>
		public static ClassMap<TModel> MapUpdatedMetaTimestamp<TModel>(this ClassMap<TModel> classMap, bool nullable = false)
			where TModel : IHaveUpdatedMeta
		{
			var updatedOnMap = classMap.Map(x => x.UpdatedOn, "UPDATED_ON").CustomType<TimestampUserType>();
			var updatedByMap = classMap.Map(x => x.UpdatedBy, "UPDATED_BY").Length(64);

			if (nullable)
			{
				updatedOnMap.Nullable();
				updatedByMap.Nullable();
			}
			else
			{
				updatedOnMap.Not.Nullable();
				updatedByMap.Not.Nullable();
			}

			return classMap;
		}


		/// <summary>
		/// Maps all properties of an <see cref="IHaveDeletedMeta"/> model
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="classMap"></param>
		/// <param name="nullable"></param>
		/// <returns></returns>
		public static ClassMap<TModel> MapDeletedMeta<TModel>(this ClassMap<TModel> classMap, bool nullable = false)
			where TModel : IHaveDeletedMeta
		{
			var deletedOnMap = classMap.Map(x => x.DeletedOn, "DELETED_ON").CustomType<NormalizedDateTimeUserType>();
			var deletedByMap = classMap.Map(x => x.DeletedBy, "DELETED_BY").Length(64);

			if (nullable)
			{
				deletedOnMap.Nullable();
				deletedByMap.Nullable();
			}
			else
			{
				deletedOnMap.Not.Nullable();
				deletedByMap.Not.Nullable();
			}

			return classMap;
		}

		/// <summary>
		/// Maps all properties of an <see cref="IHaveDeletedMeta"/> model using TimestampWTimeZoneUserType
		/// for DateTimeOffset properties
		/// </summary>
		/// <param name="classMap"></param>
		/// <param name="nullable"></param>
		/// <typeparam name="TModel"></typeparam>
		/// <returns></returns>
		public static ClassMap<TModel> MapDeletedMetaTimestamp<TModel>(this ClassMap<TModel> classMap, bool nullable = false)
			where TModel : IHaveDeletedMeta
		{
			var deletedOnMap = classMap.Map(x => x.DeletedOn, "DELETED_ON").CustomType<TimestampUserType>();
			var deletedByMap = classMap.Map(x => x.DeletedBy, "DELETED_BY").Length(64);

			if (nullable)
			{
				deletedOnMap.Nullable();
				deletedByMap.Nullable();
			}
			else
			{
				deletedOnMap.Not.Nullable();
				deletedByMap.Not.Nullable();
			}

			return classMap;
		}

		/// <summary>
		/// Maps all properties of an <see cref="IHaveCreatedMeta"/>, <see cref="IHaveUpdatedMeta"/> and <see cref="IHaveDeletedMeta"/> model
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="classMap"></param>
		/// <param name="nullableCreatedMeta"></param>
		/// <param name="nullableUpdatedMeta"></param>
		/// <param name="nullableDeletedMeta"></param>
		/// <returns></returns>
		public static ClassMap<TModel> MapAllMeta<TModel>(this ClassMap<TModel> classMap, bool nullableCreatedMeta = false, bool nullableUpdatedMeta = false, bool nullableDeletedMeta = false)
			where TModel : IHaveCreatedMeta, IHaveUpdatedMeta, IHaveDeletedMeta
		{
			return
				classMap
					.MapCreatedMeta(nullableCreatedMeta)
					.MapUpdatedMeta(nullableUpdatedMeta)
					.MapDeletedMeta(nullableDeletedMeta);
		}

		/// <summary>
		/// Maps all properties of an <see cref="IHaveCreatedMeta"/>, <see cref="IHaveUpdatedMeta"/> and <see cref="IHaveDeletedMeta"/> model using TimestampWTimeZoneUserType
		/// for DateTimeOffset properties
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="classMap"></param>
		/// <param name="nullableCreatedMeta"></param>
		/// <param name="nullableUpdatedMeta"></param>
		/// <param name="nullableDeletedMeta"></param>
		/// <returns></returns>
		public static ClassMap<TModel> MapAllMetaTimestamp<TModel>(this ClassMap<TModel> classMap, bool nullableCreatedMeta = false, bool nullableUpdatedMeta = false, bool nullableDeletedMeta = false)
			where TModel : IHaveCreatedMeta, IHaveUpdatedMeta, IHaveDeletedMeta
		{
			return
				classMap
					.MapCreatedMetaTimestamp(nullableCreatedMeta)
					.MapUpdatedMetaTimestamp(nullableUpdatedMeta)
					.MapDeletedMetaTimestamp(nullableDeletedMeta);
		}

		/// <summary>
		/// Maps all properties of an <see cref="IHaveVersionAsLong"/> model
		/// </summary>
		/// <param name="classMap"></param>
		/// <param name="nullable"></param>
		/// <typeparam name="TModel"></typeparam>
		/// <returns></returns>
		public static ClassMap<TModel> MapVersion<TModel>(this ClassMap<TModel> classMap, bool nullable = false)
			where TModel : IHaveVersionAsLong
		{
			var versionMap = classMap.Version(e => e.Version).Column("VERSION").Generated.Never().UnsavedValue("0");

			if (nullable)
			{
				versionMap.Nullable();
			}
			else
			{
				versionMap.Not.Nullable();
			}

			classMap.OptimisticLock.Version();

			return classMap;
		}
	}
}
