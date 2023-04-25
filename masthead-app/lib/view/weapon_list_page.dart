import 'dart:collection';

import 'package:flutter/material.dart';
import 'package:graphql_flutter/graphql_flutter.dart';
import 'package:masthead/view/weapon_detail_page.dart';

import '../model/common.dart';
import '../model/weapon.dart';

class WeaponListPage extends StatefulWidget {
  const WeaponListPage({super.key});

  @override
  State<WeaponListPage> createState() => _WeaponListPageState();

  static String _factionToString(FactionDefinition faction) {
    switch (faction) {
      case FactionDefinition.none:
        return 'Usable by all';
      case FactionDefinition.vs:
        return 'Vanu Sovereignty';
      case FactionDefinition.nc:
        return 'New Conglomerate';
      case FactionDefinition.tr:
        return 'Terran Republic';
      case FactionDefinition.nso:
        return 'Nanite Systems Operatives';
      default:
        return 'Unknown faction';
    }
  }
}

class _WeaponListPageState extends State<WeaponListPage> {
  static const String allValue = 'all';
  static const String uncategorisedLabel = 'Uncategorised';

  final _searchTextController = TextEditingController();
  String? _selectedFaction;
  String? _selectedCategory;
  String _searchValue = '';

  void onFactionSelected(String? value) {
    if (value is String) {
      setState(() => _selectedFaction = value);
    }
  }

  void onCategorySelected(String? value) {
    if (value is String) {
      setState(() => _selectedCategory = value);
    }
  }

  void onSearchChanged(String newValue) {
    setState(() => _searchValue = newValue.toLowerCase());
  }

  void clearSearch() {
    _searchTextController.clear();
    setState(() => _searchValue = '');
  }

  void navigateToWeaponDetail(int weaponItemId, BuildContext context) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => WeaponDetailPage(weaponItemId: weaponItemId),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    String getWeaponsGql = """
query Weapons {
  weapons {
    itemId
    weaponId
    faction
    name
    itemCategoryName
  }
}
""";

    return Scaffold(
      appBar: AppBar(title: const Text('Masthead')),
      body: Query(
        options: QueryOptions(document: gql(getWeaponsGql)),
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

          var weapons = <Weapon>[];
          var individualFactions = HashSet<FactionDefinition>();
          var individualCategories = HashSet<String>();

          for (dynamic dynWeapon in result.data!['weapons'] as List) {
            var weapon = Weapon.fromJson(dynWeapon);
            weapons.add(weapon);

            individualFactions.add(weapon.faction);
            individualCategories
                .add(weapon.itemCategoryName ?? uncategorisedLabel);
          }

          var factions = <FactionDefinition>[];
          var categories = <String>[];

          factions.addAll(individualFactions);
          factions.sort((a, b) => a.index.compareTo(b.index));

          categories.addAll(individualCategories);
          categories.sort((a, b) => a.compareTo(b));

          weapons.sort((a, b) {
            if (a.name == null && b.name == null) {
              return 0;
            }

            if (a.name == null) {
              return 1;
            }

            if (b.name == null) {
              return -1;
            }

            return a.name!.compareTo(b.name!);
          });

          var listElements = <Widget>[];
          for (var weapon in weapons) {
            if (weapon.name == null || weapon.name!.startsWith('PLACEHOLDER')) {
              continue;
            }

            String category = weapon.itemCategoryName ?? uncategorisedLabel;

            if (_selectedFaction != allValue &&
                _selectedFaction != null &&
                weapon.faction.name != _selectedFaction) {
              continue;
            }

            if (_selectedCategory != allValue &&
                _selectedCategory != null &&
                category != _selectedCategory) {
              continue;
            }

            if (_searchValue != '' &&
                weapon.name!.toLowerCase().contains(_searchValue) == false) {
              continue;
            }

            listElements.add(
              ListTile(
                title: Text(weapon.name ?? weapon.itemId.toString()),
                subtitle: Text(WeaponListPage._factionToString(weapon.faction)),
                enabled: true,
                onTap: () => navigateToWeaponDetail(weapon.itemId, context),
              ),
            );
          }

          return Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Padding(
                padding: const EdgeInsets.only(left: 10, right: 10),
                child: Wrap(children: [
                  // Search bar
                  TextField(
                    controller: _searchTextController,
                    decoration: InputDecoration(
                      prefixIcon: const Icon(Icons.search),
                      suffixIcon: IconButton(
                        onPressed: clearSearch,
                        icon: const Icon(Icons.clear),
                      ),
                      hintText: 'Search...',
                    ),
                    onChanged: onSearchChanged,
                  ),

                  // Faction selector
                  DropdownButton(
                    hint: const Text('Filter by faction'),
                    items: [
                      const DropdownMenuItem(
                        value: allValue,
                        child: Text('All'),
                      ),
                      for (FactionDefinition faction in factions)
                        DropdownMenuItem(
                          value: faction.name,
                          child: Text(WeaponListPage._factionToString(faction)),
                        )
                    ],
                    value: _selectedFaction,
                    onChanged: onFactionSelected,
                  ),

                  const SizedBox(width: 10),

                  // Item category selector
                  DropdownButton(
                    hint: const Text('Filter by category'),
                    items: [
                      const DropdownMenuItem(
                        value: allValue,
                        child: Text('All'),
                      ),
                      for (String category in categories)
                        DropdownMenuItem(value: category, child: Text(category))
                    ],
                    value: _selectedCategory,
                    onChanged: onCategorySelected,
                  ),
                ]),
              ),

              // Weapon list
              Expanded(child: ListView(children: listElements))
            ],
          );
        },
      ),
    );
  }
}
