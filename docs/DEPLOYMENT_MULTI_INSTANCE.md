# Deployment Multi-Instância e Load Balancer

Este documento descreve como configurar e fazer deploy da aplicação Araponga em múltiplas instâncias com load balancer.

## 📋 Pré-requisitos

- API stateless (sem sessão no servidor)
- Cache distribuído (Redis) configurado
- Banco de dados com suporte a read replicas (opcional, mas recomendado)
- Load balancer (Nginx, AWS ALB, Azure Load Balancer, etc.)

## 🔧 Configuração

### 1. API Stateless

A API Araponga é **stateless** por design:
- Autenticação via JWT (sem sessão no servidor)
- Cache distribuído via Redis (ou fallback para IMemoryCache)
- Sem sticky sessions necessárias

### 2. Load Balancer

#### Nginx (Exemplo)

```nginx
upstream araponga_backend {
    least_conn;  # Balanceamento por menor conexão
    server api1.example.com:5000;
    server api2.example.com:5000;
    server api3.example.com:5000;
}

server {
    listen 80;
    server_name api.araponga.com;

    location / {
        proxy_pass http://araponga_backend;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

#### AWS Application Load Balancer

1. Criar Target Group com health checks
2. Registrar instâncias EC2
3. Configurar listener na porta 80/443
4. Health check path: `/health`

#### Azure Load Balancer

1. Criar Load Balancer
2. Adicionar backend pool com VMs
3. Configurar health probe
4. Adicionar load balancing rules

### 3. Health Checks

A API expõe endpoint de health check:

```
GET /health
```

Configure o load balancer para usar este endpoint.

### 4. Variáveis de Ambiente

Cada instância precisa das seguintes variáveis:

```bash
# Database (write)
ConnectionStrings__Postgres=Host=db.example.com;Database=araponga;Username=...

# Database (read replica - opcional)
# Para usar read replicas, configure uma connection string separada e use
# ArapongaDbContext com ChangeTracker.QueryTrackingBehavior = NoTracking
ConnectionStrings__PostgresReadOnly=Host=db-read.example.com;Database=araponga;Username=...

# Redis (opcional, fallback para IMemoryCache)
ConnectionStrings__Redis=redis.example.com:6379

# JWT
JWT__SIGNINGKEY=your-secret-key-here

# CORS
Cors__AllowedOrigins__0=https://app.araponga.com
Cors__AllowedOrigins__1=https://www.araponga.com
```

### 5. Deploy Multi-Instância

#### Docker Compose (Exemplo)

```yaml
version: '3.8'

services:
  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
    depends_on:
      - api1
      - api2
      - api3

  api1:
    build: .
    environment:
      - ConnectionStrings__Postgres=${POSTGRES_CONNECTION}
      - ConnectionStrings__Redis=${REDIS_CONNECTION}
    depends_on:
      - postgres
      - redis

  api2:
    build: .
    environment:
      - ConnectionStrings__Postgres=${POSTGRES_CONNECTION}
      - ConnectionStrings__Redis=${REDIS_CONNECTION}
    depends_on:
      - postgres
      - redis

  api3:
    build: .
    environment:
      - ConnectionStrings__Postgres=${POSTGRES_CONNECTION}
      - ConnectionStrings__Redis=${REDIS_CONNECTION}
    depends_on:
      - postgres
      - redis

  postgres:
    image: postgres:16
    environment:
      - POSTGRES_DB=araponga
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=${POSTGRES_PASSWORD}

  redis:
    image: redis:7-alpine
```

#### Kubernetes (Exemplo)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: araponga-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: araponga-api
  template:
    metadata:
      labels:
        app: araponga-api
    spec:
      containers:
      - name: api
        image: araponga/api:latest
        env:
        - name: ConnectionStrings__Postgres
          valueFrom:
            secretKeyRef:
              name: araponga-secrets
              key: postgres-connection
        - name: ConnectionStrings__Redis
          valueFrom:
            secretKeyRef:
              name: araponga-secrets
              key: redis-connection
        ports:
        - containerPort: 5000
        livenessProbe:
          httpGet:
            path: /health
            port: 5000
          initialDelaySeconds: 30
          periodSeconds: 10
---
apiVersion: v1
kind: Service
metadata:
  name: araponga-api-service
spec:
  selector:
    app: araponga-api
  ports:
  - port: 80
    targetPort: 5000
  type: LoadBalancer
```

## ✅ Validação

### 1. Testar Stateless

```bash
# Fazer request para instância 1
curl -H "Authorization: Bearer <token>" http://api1/health

# Fazer request para instância 2 (mesmo token deve funcionar)
curl -H "Authorization: Bearer <token>" http://api2/health
```

### 2. Testar Load Balancing

```bash
# Fazer múltiplas requests e verificar que são distribuídas
for i in {1..10}; do
  curl http://loadbalancer/health
done
```

### 3. Testar Health Checks

```bash
# Verificar que o load balancer detecta instâncias saudáveis
curl http://loadbalancer/health
```

## 📊 Monitoramento

- **Health Checks**: Monitorar `/health` endpoint
- **Logs**: Centralizar logs de todas as instâncias
- **Métricas**: Coletar métricas de cada instância
- **Cache Hit Rate**: Monitorar métricas de cache Redis

## 🔒 Segurança

- **HTTPS**: Sempre usar HTTPS em produção
- **Rate Limiting**: Configurar rate limiting no load balancer também
- **Firewall**: Restringir acesso ao banco de dados apenas para instâncias da API

## 📝 Notas

- **Sticky Sessions**: Não são necessárias devido à natureza stateless da API
- **Cache**: Redis é recomendado para cache distribuído, mas IMemoryCache funciona como fallback
- **Read Replicas**: Opcional, mas recomendado para melhorar performance de leitura
