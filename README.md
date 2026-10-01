# shopping-app
                         ┌──────────────────────────┐
                         │      Web / Mobile UI     │
                         └────────────┬─────────────┘
                                      │
                                      │ HTTPS
                                      ▼
                         ┌──────────────────────────┐
                         │      Ocelot API Gateway  │
                         │                          │
                         │ • Routing                │
                         │ • CORS                   │
                         │ • Authentication        │
                         │ • Authorization         │
                         │ • Caching               │
                         │ • Rate Limiting*        │
                         └────────────┬─────────────┘
                                      │
                    ┌─────────────────┼─────────────────┐
                    │                 │                 │
                    ▼                 ▼                 ▼
             ┌────────────┐    ┌────────────┐    ┌────────────┐
             │  Identity  │    │  Catalog   │    │   Basket   │
             │  Service   │    │  Service   │    │  Service   │
             └─────┬──────┘    └─────┬──────┘    └─────┬──────┘
                   │                 │                 │
                   ▼                 ▼                 ▼
             ┌────────────┐    ┌────────────┐    ┌────────────┐
             │ SQL Server │    │  MongoDB   │    │   Redis    │
             └────────────┘    └────────────┘    └────────────┘

                    ┌─────────────────┼─────────────────┐
                    │                                   │
                    ▼                                   ▼
             ┌────────────┐                      ┌────────────┐
             │   Order    │                      │  Discount  │
             │  Service   │                      │  Service   │
             └─────┬──────┘                      └─────┬──────┘
                   │                                   │
                   ▼                                   ▼
             ┌────────────┐                      ┌────────────┐
             │ PostgreSQL │                      │ PostgreSQL │
             └────────────┘                      └────────────┘


                   Asynchronous Communication
                              │
                              ▼
                       ┌─────────────┐
                       │  RabbitMQ   │
                       └─────────────┘


════════════════════ CENTRALIZED LOGGING ════════════════════

 Identity ───────┐
 Catalog ────────┤
 Basket ─────────┤
 Ordering ───────┤──→ Shared Serilogger ──→ Elasticsearch
 Discount ───────┤                              │
 Ocelot Gateway ─┘                              ▼
                                            ┌─────────┐
                                            │ Kibana  │
                                            └─────────┘
