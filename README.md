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


Steps to spin-up containers locally in docker-desktop:
------------------------------------------------------
docker compose build catalog.api basket.api ordering.api discount.api identitymanagement.api ocelot.api --no-cache
docker compose up portainer catalogdb mongo-express basketdb discountdb pgadmin shoppingdb redisinsight rabbitmq elasticsearch kibana grafana loki
docker compose up catalog.api basket.api ordering.api discount.api identitymanagement.api ocelot.api -d
