import 'package:english_words/english_words.dart';
import 'package:flutter/material.dart';
import 'package:graphql_flutter/graphql_flutter.dart';
import 'package:masthead/view/weapon_list_page.dart';
import 'package:provider/provider.dart';

const graphqlHost = 'census.lithafalcon.cc/masthead'; //'localhost:7080';
const graphqlEndpoint = 'https://$graphqlHost/graphql';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    ValueNotifier<GraphQLClient> client = ValueNotifier(
      GraphQLClient(
        link: HttpLink(graphqlEndpoint),
        cache: GraphQLCache(store: InMemoryStore()),
      ),
    );

    return GraphQLProvider(
      client: client,
      child: ChangeNotifierProvider(
        create: (context) => MyAppState(),
        child: MaterialApp(
          title: 'Namer App',
          theme: ThemeData(
            useMaterial3: true,
            //colorScheme: ColorScheme.fromSeed(seedColor: Colors.pink),
            primarySwatch: Colors.cyan,
          ),
          home: const WeaponListPage(),
        ),
      ),
    );
  }
}

class MyAppState extends ChangeNotifier {
  WordPair current = WordPair.random();
  var favourites = <WordPair>[];

  void getNext() {
    current = WordPair.random();
    notifyListeners();
  }

  void toggleFavourite() {
    if (favourites.contains(current)) {
      favourites.remove(current);
    } else {
      favourites.add(current);
    }

    notifyListeners();
  }

  void removeFavourite(WordPair pair) {
    if (!favourites.contains(pair)) {
      return;
    }

    favourites.remove(pair);
    notifyListeners();
  }
}
