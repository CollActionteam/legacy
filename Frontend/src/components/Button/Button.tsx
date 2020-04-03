import React from 'react';
import styles from './Button.module.scss';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Link } from 'react-router-dom';

export const Button = ({ children, variant = 'primary', ...props }: any) => {
  // External link
  if (props.url) {
    return (
      <a
        className={styles[variant]}
        href={props.url}
        target="_blank"
        rel="noopener noreferrer"
      >
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

export const SecondaryButton = ({ children, ...props }: any) => (
  <Button variant="secondary" {...props}>
    {children}
  </Button>
);

export const TertiaryButton = ({ children, ...props }: any) => (
  <Button variant="tertiary" {...props}>
    {children}
  </Button>
);

export const GhostButton = ({ children, ...props }: any) => (
  <Button variant="ghost" {...props}>
    {children}
  </Button>
);

export const SecondaryGhostButton = ({ children, ...props }: any) => (
  <Button variant="ghostSecondary" {...props}>
    {children}
  </Button>
);

export const IconButton = ({ ...props }) => (
  <Button variant="icon" {...props}>
    <FontAwesomeIcon icon={props.icon}></FontAwesomeIcon>
  </Button>
);

export const CircleButtonContainer = ({ children }: any) => (
  <div className={styles.circleContainer}>{children}</div>
);

export const CircleButton = ({ children, ...props }: any) => (
  <Button variant="circle" {...props}>
    <span className={styles.circleContent}>{children}</span>
  </Button>
);
