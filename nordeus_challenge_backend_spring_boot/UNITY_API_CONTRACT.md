# Unity API Contract

Ovaj dokument je praktican ugovor za Unity integraciju sa backend API-jem.

## Base URL

- Lokalno: http://localhost:8080

## Error format (za sve endpoint-e)

Backend vraca standardizovan error JSON:

{
  "timestamp": "2026-04-21T13:11:12.145",
  "status": 400,
  "error": "Bad Request",
  "code": "VALIDATION_FAILED",
  "message": "Validation failed",
  "path": "/api/battle-sessions",
  "details": [
    "heroId: heroId is required",
    "monsterId: monsterId is required"
  ]
}

Kodovi gresaka:

- RESOURCE_NOT_FOUND
- BAD_REQUEST
- VALIDATION_FAILED
- INTERNAL_ERROR

## 1) Ucitaj heroje

- Method: GET
- Path: /api/heroes

Primer response:

[
  {
    "id": 1,
    "name": "Knight of Dawn",
    "maxHealth": 120,
    "currentHealth": 120,
    "attack": 18,
    "defense": 14,
    "magic": 10,
    "moves": [
      {
        "id": 1,
        "name": "Slash",
        "moveType": "PHYSICAL",
        "effectType": "DAMAGE",
        "power": 14,
        "buffAttribute": null,
        "buffAmount": 0,
        "buffDuration": 0
      }
    ]
  }
]

## 2) Ucitaj monstrume

- Method: GET
- Path: /api/monsters

Primer response:

[
  {
    "id": 1,
    "name": "Witch",
    "difficultyRank": 1,
    "maxHealth": 80,
    "attack": 10,
    "defense": 8,
    "magic": 18
  }
]

## 3) Kreiraj battle session

- Method: POST
- Path: /api/battle-sessions
- Content-Type: application/json

Request:

{
  "heroId": 1,
  "monsterId": 3
}

Response:

{
  "sessionId": "c4b19f1c-3f7f-4f22-8cd2-4d82a5fd6bd0",
  "hero": {
    "id": 1,
    "name": "Knight of Dawn",
    "maxHealth": 120,
    "currentHealth": 120,
    "attack": 18,
    "defense": 14,
    "magic": 10,
    "moves": [
      {
        "id": 1,
        "name": "Slash",
        "moveType": "PHYSICAL",
        "effectType": "DAMAGE",
        "power": 14,
        "buffAttribute": null,
        "buffAmount": 0,
        "buffDuration": 0
      }
    ]
  },
  "monster": {
    "id": 3,
    "name": "Giant Spider",
    "difficultyRank": 3,
    "maxHealth": 130,
    "attack": 20,
    "defense": 15,
    "magic": 14
  },
  "battleState": {
    "heroId": 1,
    "monsterId": 3,
    "selectedMoveId": null,
    "heroCurrentHealth": 120,
    "heroAttack": 18,
    "heroDefense": 14,
    "heroMagic": 10,
    "monsterCurrentHealth": 130,
    "monsterAttack": 20,
    "monsterDefense": 15,
    "monsterMagic": 14,
    "activeBuffs": []
  }
}

## 4) Ucitaj session state

- Method: GET
- Path: /api/battle-sessions/{sessionId}

Primer:

GET /api/battle-sessions/c4b19f1c-3f7f-4f22-8cd2-4d82a5fd6bd0

Response je isti shape kao create session response.

## 5) Odigraj jedan turn

- Method: POST
- Path: /api/battle-sessions/{sessionId}/turn
- Content-Type: application/json

Request:

{
  "selectedMoveId": 1
}

Response:

{
  "heroId": 1,
  "monsterId": 3,
  "heroCurrentHealth": 97,
  "heroAttack": 18,
  "heroDefense": 14,
  "heroMagic": 10,
  "monsterCurrentHealth": 113,
  "monsterAttack": 20,
  "monsterDefense": 15,
  "monsterMagic": 14,
  "heroMoveName": "Slash",
  "monsterMoveName": "Poison Bite",
  "battleFinished": false,
  "winner": "NONE",
  "logs": [
    "Knight of Dawn used Slash for 17 effect value.",
    "Giant Spider used Poison Bite for 23 effect value."
  ],
  "activeBuffs": []
}

## 6) Obrisi session

- Method: DELETE
- Path: /api/battle-sessions/{sessionId}
- Response: 204 No Content

## Unity flow (preporuka)

1. Pozovi GET /api/heroes i GET /api/monsters.
2. Nakon odabira, pozovi POST /api/battle-sessions.
3. Cuvaj sessionId lokalno u BattleManager-u.
4. Za svaki potez salji POST /api/battle-sessions/{sessionId}/turn.
5. Ako battleFinished postane true, prikazi rezultat i pozovi DELETE /api/battle-sessions/{sessionId}.
6. Za greske granaj po polju code, a details koristi za UI poruke.

## Napomene

- Session je in-memory i ima TTL cleanup.
- Default TTL je 30 minuta.
- Ako session istekne, backend vraca RESOURCE_NOT_FOUND za trazeni sessionId.
- Swagger UI: /swagger-ui.html
- OpenAPI docs: /v3/api-docs
