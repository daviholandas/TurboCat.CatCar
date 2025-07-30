# 🔒 Security & Quality Analysis Dashboard

[![CI/CD Pipeline](https://github.com/daviholandas/TurboCat.CatCar/workflows/CI%2FCD%20Pipeline/badge.svg)](https://github.com/daviholandas/TurboCat.CatCar/actions)
[![CodeQL](https://github.com/daviholandas/TurboCat.CatCar/workflows/CodeQL/badge.svg)](https://github.com/daviholandas/TurboCat.CatCar/security/code-scanning)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=YOUR_PROJECT_KEY&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=YOUR_PROJECT_KEY)
[![codecov](https://codecov.io/gh/daviholandas/TurboCat.CatCar/branch/master/graph/badge.svg)](https://codecov.io/gh/daviholandas/TurboCat.CatCar)

## 📊 Analysis Reports

| Analysis Type | Status | Report Location | Description |
|---------------|--------|-----------------|-------------|
| **CodeQL Security Analysis** | 🔍 | [Security Tab → Code Scanning](../../security/code-scanning) | Static Application Security Testing (SAST) |
| **Trivy Vulnerability Scan** | 🛡️ | [Security Tab → Code Scanning](../../security/code-scanning) | Software Composition Analysis (SCA) |
| **SonarQube Quality Gate** | 📊 | [SonarCloud Dashboard](https://sonarcloud.io) | Code Quality & Security Hotspots |
| **Test Coverage** | 📈 | [Codecov Dashboard](https://codecov.io) | Unit Test Coverage Reports |

## 🚀 Quick Access Links

### Security Reports
- 🔒 **[Security Overview](../../security)** - Central security dashboard
- 🚨 **[Security Alerts](../../security/advisories)** - Vulnerability advisories
- 🔍 **[Code Scanning](../../security/code-scanning)** - CodeQL & Trivy results
- 🔐 **[Secret Scanning](../../security/secret-scanning)** - Credential leak detection

### Quality Reports
- ✅ **[Actions Dashboard](../../actions)** - CI/CD pipeline status
- 📊 **[Pull Requests](../../pulls)** - Code review & quality checks
- 📈 **[Insights](../../pulse)** - Repository activity and metrics

## 🛠️ Configured Analysis Tools

### CodeQL (GitHub Advanced Security)
- **Language**: C# (.NET 9.0)
- **Query Suites**: Security Extended, Security and Quality
- **Frequency**: Every push/PR
- **Integration**: GitHub Security tab

### Trivy Scanner
- **Scope**: Dependencies, configuration files
- **Severity**: CRITICAL, HIGH, MEDIUM
- **Format**: SARIF (uploaded to GitHub)
- **Frequency**: Every push/PR

### SonarQube Analysis
- **Platform**: SonarCloud
- **Metrics**: Quality Gate, Coverage, Technical Debt
- **Integration**: CI/CD pipeline
- **Frequency**: Every push/PR

## 📋 Quality Metrics Overview

The CI/CD pipeline automatically generates and publishes the following reports:

1. **Security Findings** - CodeQL identifies potential security vulnerabilities
2. **Dependency Vulnerabilities** - Trivy scans for known CVEs in dependencies  
3. **Code Quality** - SonarQube analyzes maintainability and reliability
4. **Test Coverage** - Codecov tracks unit test coverage across the codebase

## 🔧 Configuration Files

- **CI/CD Pipeline**: [`.github/workflows/ci-cd.yml`](../../blob/master/.github/workflows/ci-cd.yml)
- **CodeQL Config**: [`.github/codeql/codeql-config.yml`](../../blob/master/.github/codeql/codeql-config.yml)
- **Security Documentation**: [`docs/security-analysis-dashboard.md`](../docs/security-analysis-dashboard.md)

---

**💡 Tip**: All security and quality reports are automatically updated on every push and pull request. Check the Security tab for the latest findings and trends.
