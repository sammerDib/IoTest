#include <gtest/gtest.h>

#include <Linspace.hpp>

TEST(Linspace, Expect_integer_values_to_be_linearized) {

  std::vector<int> actual = linspace(0, 10, 6);
  std::vector<int> expected{0, 2, 4, 6, 8, 10};
  ASSERT_EQ(expected.size(), actual.size());
  ASSERT_EQ(expected, actual);
}
TEST(Linspace, Expect_double_values_to_be_linearized) {

  std::vector<double> actual = linspace(0.0, 1.0, 5);
  std::vector<double> expected{0, 0.25, 0.5, 0.75, 1};
  ASSERT_EQ(expected.size(), actual.size());
  for (size_t index = 0; index < expected.size(); ++index) {
    ASSERT_DOUBLE_EQ(expected.at(index), actual.at(index));
  }
}
