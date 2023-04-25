import 'package:flutter/material.dart';
import 'package:graphql_flutter/graphql_flutter.dart';
import 'package:masthead/model/weapon_full.dart';

class WeaponDetailPage extends StatelessWidget {
  final int weaponItemId;

  const WeaponDetailPage({super.key, required this.weaponItemId});

  @override
  Widget build(BuildContext context) {
    String getWeaponDetailGql = """
query Weapon {
  weapon(weaponItemId: $weaponItemId) {
    itemId,
    name,
    description,
    imagePath,
    equipMs,
    unequipMs,
    ammoSlots {
      clipSize,
      capacity
    },
    heatBleedOffRate
  }
}
""";

    return Query(
      options: QueryOptions(document: gql(getWeaponDetailGql)),
      builder: (QueryResult result,
          {VoidCallback? refetch, FetchMore? fetchMore}) {
        if (result.hasException) {
          return Text(result.exception.toString());
        }

        if (result.isLoading) {
          return const Center(child: CircularProgressIndicator(value: null));
        }

        if (result.data == null) {
          return const Center(child: Text('No weapons found'));
        }

        var weapon = WeaponFull.fromJson(result.data!['weapon']);

        return Scaffold(
          appBar: AppBar(title: Text(weapon.name ?? 'Unknown Weapon')),
          body: ListView(
            padding: const EdgeInsets.fromLTRB(8, 4, 8, 4),
            children: [
              Card(
                child: Column(
                  children: [
                    if (weapon.imagePath != null)
                      Image.network(
                          'https://census.daybreakgames.com${weapon.imagePath!}'),
                    ListTile(
                      title: Text(weapon.name ?? weapon.itemId.toString()),
                      subtitle: Text(weapon.description ?? 'No description'),
                    ),
                  ],
                ),
              ),
              ExpansionTile(
                title: const Text('Basic Stats'),
                initiallyExpanded: true,
                children: [
                  Table(
                    children: [
                      TableRow(children: [
                        const Text('Equip Time'),
                        Text('${weapon.equipMs}ms')
                      ]),
                      TableRow(children: [
                        const Text('Unequip Time'),
                        Text('${weapon.unequipMs}ms')
                      ]),
                    ],
                  )
                ],
              ),
              ExpansionTile(
                title: const Text('Ammo Slots'),
                children: [
                  ListView(
                    shrinkWrap: true,
                    children: [
                      for (AmmoSlot slot in weapon.ammoSlots)
                        ListTile(
                          title: Text('${slot.clipSize} / ${slot.capacity}'),
                        )
                    ],
                  ),
                ],
              )
            ],
          ),
        );
      },
    );
  }
}
