class WeaponFull {
  final int itemId;
  final String? name;
  final String? description;
  final String? imagePath;
  final int equipMs;
  final int unequipMs;
  final List<AmmoSlot> ammoSlots;

  WeaponFull(
    this.itemId,
    this.name,
    this.description,
    this.imagePath,
    this.equipMs,
    this.unequipMs,
    this.ammoSlots,
  );

  factory WeaponFull.fromJson(dynamic data) {
    return WeaponFull(
      data['itemId'],
      data['name'],
      data['description'],
      data['imagePath'],
      data['equipMs'],
      data['unequipMs'],
      (data['ammoSlots'] as List).map((e) => AmmoSlot.fromJson(e)).toList(),
    );
  }
}

class AmmoSlot {
  final int capacity;
  final int clipSize;

  AmmoSlot(this.capacity, this.clipSize);

  factory AmmoSlot.fromJson(dynamic data) {
    return AmmoSlot(data['capacity'], data['clipSize']);
  }
}
