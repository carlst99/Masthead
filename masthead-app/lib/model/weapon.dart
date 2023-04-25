import 'package:masthead/model/common.dart';

class Weapon {
  final int itemId;
  final FactionDefinition faction;
  final String? name;
  final String? itemCategoryName;

  Weapon(
    this.itemId,
    this.faction,
    this.name,
    this.itemCategoryName,
  );

  factory Weapon.fromJson(dynamic data) {
    return Weapon(
        data['itemId'],
        FactionDefinition.values.firstWhere(
          (f) => f.name == data['faction'].toString().toLowerCase(),
          orElse: () => FactionDefinition.none,
        ),
        (data['name'] as String?)?.trim(),
        data['itemCategoryName']);
  }
}
