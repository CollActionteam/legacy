import React from "react";
import { render } from "@testing-library/react";
import { Button } from "./Button";

describe("Button", () => {
  it(`renders a button with children`, () => {
    const children = "This is a button";
    const { getByText } = render(<Button>{children}</Button>);

    const button = getByText(children);

    expect(button).toBeInTheDocument();
  });
});
