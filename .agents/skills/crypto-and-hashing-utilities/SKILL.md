---
name: "crypto-and-hashing-utilities"
description: "Uso correto de algoritmos criptográficos modernos: Argon2id, bcrypt com salt, AES-256-GCM para dados em repouso e CSPRNG."
id: "SKL-057"
title: "Skill: Criptografia Moderna & Hashing Seguro"
date: "2026-08-27"
last_updated: "2026-08-27"
tags: ['skill', 'seguranca', 'backend', 'bcrypt']
category: "skills"
scope: "global"
project: null
status: "vigente"
evidence: "inferido"
verified_at: "2026-08-27"
score: 10
archived: false
---

# 🔑 Cryptography & Secure Hashing

## 🎯 Objetivo
Aplicar algoritmos criptográficos seguros e impedir o uso de primitivas obsoletas (MD5, SHA1, DES).

---

## 🛡️ Algoritmos Obrigatórios
1. **Senhas**: Usar `Argon2id` (preferencial) ou `bcrypt` com fator de custo ≥ 12.
2. **Criptografia Simétrica**: Usar `AES-256-GCM` com vetor de inicialização (IV) único de 96 bits para cada operação.
3. **Geração de Aleatórios**: Usar geradores criptograficamente seguros (`crypto.randomBytes` / `RandomNumberGenerator`).
