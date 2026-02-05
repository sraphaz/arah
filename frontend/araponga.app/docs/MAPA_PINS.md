# Mapa e pins – App Ará

Estado atual e recomendação de componente para visualização de pins e interação com o mapa.

---

## Estado atual

- **No app Flutter:** não há tela de mapa nem exibição de pins. A tela **Explorar** hoje mostra apenas a lista de territórios (lista, sem mapa).
- **No backend:** a API está pronta: `GET /api/v1/map/pins`, `GET /api/v1/map/entities`, etc. (ver `docs/api/60_06_API_MAPA.md` e `docs/funcional/05_MAPA_TERRITORIAL.md`).
- **BFF:** a jornada **map** pode expor esses endpoints; ao implementar o mapa no app, as chamadas devem passar pelo BFF.

---

## Recomendação de componente

Para **visualização de pins no mapa e interação** (zoom, arrastar, toque em pin, etc.), as opções mais adequadas em Flutter são:

| Pacote | Prós | Contras |
|--------|------|---------|
| **google_maps_flutter** | Oficial Google, markers nativos, boa interação, documentação sólida | Requer API key e conta Google; uso sujeito a termos e custos em alto volume |
| **flutter_map** | Open source, OpenStreetMap, várias fontes de tiles (Mapbox, etc.), suporte a offline, marcadores e camadas | Configuração de tiles e estilos; não é “Google Maps” |

**Recomendação:**

- **Se a marca e a base de mapas forem Google:** use **google_maps_flutter**. Markers com `Marker(position: LatLng(...))`, `onTap`, e `CameraPosition` cobrem bem pins e navegação.
- **Se quiser evitar dependência da Google ou precisar de offline/múltiplas fontes:** use **flutter_map** (ex.: `flutter_map` + `latlong2`). Permite `MarkerLayer` com pins, gestos de mapa e troca de provedor de tiles.

Ambos permitem boa **visualização dos pins e interação com o mapa** (arrastar, zoom, toque em pin). A escolha é principalmente por ecossistema (Google vs open source) e requisitos de licença/custo.

---

## Próximos passos para implementar

1. Expor no BFF a jornada **map** (ex.: `GET map/pins?territoryId=...`) consumindo a API existente.
2. No app: adicionar dependência **google_maps_flutter** ou **flutter_map** em `pubspec.yaml`.
3. Criar feature **map** (tela + provider que chama o BFF), exibir mapa com `CameraPosition` centrada no território/usuário e desenhar pins a partir da resposta de pins/entities.
4. Opcional: link Explorar ↔ Mapa (aba ou botão “Ver no mapa”) e deep link para abrir o mapa em um território.

Referências no repositório: `docs/funcional/05_MAPA_TERRITORIAL.md`, `docs/api/60_06_API_MAPA.md`.
