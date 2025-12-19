# Tests - AcademyIODevops

Estrutura de testes unitários para a solução AcademyIODevops.

## 📊 Status Atual

### Courses API ✅
- **99 testes implementados**
- **100% de sucesso**
- **Cobertura**: Domain, Application (Handlers, Validators)

## 🏗️ Estrutura Implementada

```
tests/
└── AcademyIODevops.Courses.API.Tests/     ✅ IMPLEMENTADO
    ├── Unit/
    │   ├── Domain/                         ✅ CourseTests, LessonTests
    │   ├── Application/
    │   │   ├── Handlers/                   ✅ Add/Update Handlers
    │   │   └── Validators/                 ✅ Command Validators
    │   ├── Builders/                       ✅ Exemplos de uso
    │   └── Fixtures/                       ✅ Exemplos de uso
    ├── Builders/                           ✅ CourseBuilder, LessonBuilder
    ├── Fixtures/                           ✅ CourseTestFixture
    ├── Integration/                        📋 Planejado (futuro)
    └── README.md                           ✅ Documentação completa
```

## 🚀 Executar Testes

### Todos os testes
```bash
dotnet test
```

### Apenas Courses API
```bash
cd tests/AcademyIODevops.Courses.API.Tests
dotnet test
```

### Com cobertura
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📈 Métricas

| Projeto | Testes | Status | Tempo |
|---------|--------|--------|-------|
| Courses.API.Tests | 99 | ✅ Pass | ~112ms |
| Auth.API.Tests | - | 📋 Planejado | - |
| Payments.API.Tests | - | 📋 Planejado | - |
| Students.API.Tests | - | 📋 Planejado | - |

## 📝 Distribuição de Testes (Courses API)

- **Domain Tests**: 22 testes
  - Course: 10 testes
  - Lesson: 12 testes

- **Application Tests**: 30 testes
  - Validators: 12 testes
  - Handlers: 18 testes

- **Builder Examples**: 11 testes

- **Fixture Examples**: 6 testes

- **Integration**: 30 testes (examples shared fixtures)

## 🎯 Próximos Passos

### Curto Prazo
1. ✅ Implementar testes para Courses API
2. 📋 Implementar testes para Auth API
3. 📋 Implementar testes para Payments API
4. 📋 Implementar testes para Students API

### Médio Prazo
1. 📋 Adicionar testes de integração (WebApplicationFactory)
2. 📋 Implementar testes de repositórios com Testcontainers
3. 📋 Configurar CI/CD com execução automática de testes
4. 📋 Configurar relatórios de cobertura

### Longo Prazo
1. 📋 Testes de performance
2. 📋 Testes de carga
3. 📋 Testes end-to-end
4. 📋 Testes de mutação

## 🛠️ Tecnologias

- **xUnit** 2.6.6 - Framework de testes
- **FluentAssertions** 6.12.0 - Assertions expressivas
- **Moq** 4.20.70 - Mocking
- **AutoFixture** 4.18.1 - Geração de dados
- **Bogus** 35.4.0 - Dados fake realistas

## 📚 Convenções

### Nomenclatura
- **Pattern**: `[Method]_Should[Behavior]_When[Condition]`
- **Exemplo**: `AddCourse_ShouldReturnTrue_WhenDataIsValid`

### Estrutura (AAA)
```csharp
[Fact]
public void Example_Test()
{
    // Arrange - Setup
    var data = CreateTestData();

    // Act - Execute
    var result = PerformAction(data);

    // Assert - Verify
    result.Should().BeTrue();
}
```

### Test Data Builders
```csharp
var course = new CourseBuilder()
    .WithName("Docker")
    .WithPrice(99.99)
    .Build();
```

## 🤝 Contribuindo

Ao adicionar novos testes:

1. Siga a estrutura de pastas estabelecida
2. Use os Builders existentes
3. Mantenha as convenções de nomenclatura
4. Documente casos complexos
5. Garanta que os testes são rápidos

## 📞 Suporte

Para dúvidas sobre testes:
- Consulte o README.md específico de cada projeto de teste
- Veja os exemplos em `BuilderUsageExamplesTests.cs`
- Veja os exemplos em `FixtureUsageExampleTests.cs`

---

**Última atualização**: 2025-12-19
**Status**: ✅ Courses API implementado - Outros serviços planejados
