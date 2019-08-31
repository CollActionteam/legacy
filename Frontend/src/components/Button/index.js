import React from "react";
import { Link } from "gatsby";
import styles from "./style.module.scss";

export const Button = ({ children, variant = "primary", onClick, to }) => {
  if(to) {
    return (
      <Link className={styles[variant]} to={to}>
        {children}
      </Link>
    );
  };

  return (
    <button className={styles[variant]} onClick={onClick}>
      {children}
    </button>
  )
};

export const SecondaryButton = ({ children, ...props }) =>
  <Button variant="secondary" {...props}>
    {children}
  </Button>;

export const TertiaryButton = ({ children, ...props }) =>
  <Button variant="tertiary" {...props}>
    {children}
  </Button>;

  export const IconButton = ({ children, ...props }) =>
    <Button variant="icon" {...props}>
      {children}
    </Button>;