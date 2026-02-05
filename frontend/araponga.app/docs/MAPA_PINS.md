# Mapa e pins – App Ará

Implementação e opções de tiles (OpenStreetMap, Mapbox).

---

## Implementado

- **App Flutter:** tela de mapa com **flutter_map** + **latlong2** (open source).
  - **Rota:** `/map` (opcional `?territoryId=...`).
  - **Explorar:** botão “Ver no mapa” na AppBar abre o mapa do território selecionado.
  - **Pins:** carregados via BFF jornada **map** (`GET map/pins?territoryId=...`). Tipos: entity, post, event, asset, alert, media.
  - **Tiles:** OpenStreetMap (padrão), com `userAgentPackageName` para conformidade com ToS.
  - **Localização:** marcador “minha localização” quando disponível; botão na AppBar para centralizar na posição atual.
- **Backend:** API `GET /api/v1/map/pins` (e entities) já existente.
- **BFF:** jornada **map** expõe `api/v1/map`; app chama `client.get('map', 'pins?territoryId=...')`.

---

## Tiles: OpenStreetMap vs Mapbox

| Fonte | Uso no app | Observação |
|--------|-------------|------------|
| **OpenStreetMap** | Padrão (atual) | Sem API key; ToS exige User-Agent identificável (`userAgentPackageName`). |
| **Mapbox** | Opcional | Melhor estilo e desempenho; requer token. Ver [Using Mapbox](https://docs.fleaflet.dev/v7/tile-servers/using-mapbox). |

Para usar **Mapbox** no lugar (ou em camada):

1. Obter token em [Mapbox](https://www.mapbox.com/).
2. Trocar/adicional `TileLayer` em `MapScreen` com `urlTemplate` Mapbox (ex.: `https://api.mapbox.com/styles/v1/mapbox/.../tiles/256/{z}/{x}/{y}@2x?access_token=YOUR_TOKEN`) e `tileSize: 512`, `zoomOffset: -1` se usar tiles 512px.
3. Manter token em variável de ambiente / config (nunca commitar).

---

## Estrutura no app

- `lib/features/map/`
  - `data/models/map_pin.dart` – modelo do pin (pinType, lat, lng, title, ids).
  - `data/repositories/map_repository.dart` – BFF GET map/pins.
  - `presentation/providers/map_pins_provider.dart` – `FutureProvider.family` por território.
  - `presentation/screens/map_screen.dart` – `FlutterMap`, `TileLayer`, `MarkerLayer`, bottom sheet no toque do pin.

---

## Android / plataforma

- **Android:** o app precisa de permissão `INTERNET` para carregar tiles. Em projetos Flutter padrão isso já vem em `android/app/src/main/AndroidManifest.xml`. Se o mapa não carregar tiles, conferir o manifest.

Referências: `docs/funcional/05_MAPA_TERRITORIAL.md`, `docs/api/60_06_API_MAPA.md`.
