import React from "react";
import { Link } from "gatsby";
import styles from "./style.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export const Button = ({ children, variant = "primary", ...props }) => {
  // External link
  if (props.url) {
    return (
      <a className={styles[variant]} href={props.url}>
        {children}
      </a>
    );
  }

  // Internal link
  if (props.to) {
    return (
      <Link className={styles[variant]} to={props.to}>
        {children}
      </Link>
    );
  }

  // onClick
  return (
    <button className={styles[variant]} {...props}>
      {children}
    </button>
  );
};

export const SecondaryButton = ({ children, ...props }) => (
  <Button variant="secondary" {...props}>
    {children}
  </Button>
);

export const TertiaryButton = ({ children, ...props }) => (
  <Button variant="tertiary" {...props}>
    {children}
  </Button>
);

export const IconButton = ({ ...props }) => (
  <Button variant="icon" {...props}>
    <FontAwesomeIcon icon={props.icon}></FontAwesomeIcon>
  </Button>
);

export const CircleButtonContainer = ({ children }) => (
  <div className={styles.circleContainer}>{children}</div>
);

export const CircleButton = ({ children, ...props }) => (
  <Button variant="circle" {...props}>
    <span className={styles.circleContent}>{children}</span>
  </Button>
);
