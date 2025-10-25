module.exports = {
  projects: [
    '<rootDir>/apps/backend/api',
    '<rootDir>/apps/frontend/bot',
    '<rootDir>/apps/frontend/mobile',
  ],
  collectCoverageFrom: [
    'src/**/*.{ts,tsx}',
    '!src/**/*.d.ts',
    '!src/**/*.test.{ts,tsx}',
    '!src/**/__tests__/**',
  ],
  coverageDirectory: '<rootDir>/coverage',
  testMatch: ['**/__tests__/**/*.test.ts', '**/__tests__/**/*.test.tsx'],
};

