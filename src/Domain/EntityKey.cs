using System.Collections.Immutable;

namespace Domain;

public class EntityKey
{
	public EntityKey(NameType name, ushort uniqueId) =>
		(Name, UniqueId, PrimaryKeyFieldList) = (Name, UniqueId, ImmutableList<Field>.Empty);

	[SetsRequiredMembers]
	public EntityKey(EntityKey other) =>
		(Name, UniqueId, Owner, PrimaryKeyFieldList) =
		(other.Name, other.UniqueId, other.Owner, other.PrimaryKeyFieldList);

	public EntityKey Add(params Field[] fields) =>
		fields.Length == 0 ? this
		: fields.Length == 1 ? new(this) { PrimaryKeyFieldList = PrimaryKeyFieldList.Add(fields[0]) }
		: new(this) { PrimaryKeyFieldList = PrimaryKeyFieldList.AddRange(fields) };

	public NameType Name { get; }
	public ushort UniqueId { get; }
	public NameType? Owner { get; init; }

	private ImmutableList<Field> PrimaryKeyFieldList { get; init; }
	public IEnumerable<Field> PrimaryKeyFields => PrimaryKeyFieldList;
}
