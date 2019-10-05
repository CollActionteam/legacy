import React from "react";
import styles from "./style.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faFacebookF,
  faTwitter,
  faLinkedinIn,
} from "@fortawesome/free-brands-svg-icons";
import { faAt } from "@fortawesome/free-solid-svg-icons";

export const Facebook = ({ url }) => {
  return (
    <a
      target="_blank"
      rel="noopener noreferrer"
      href={`https://www.facebook.com/sharer/sharer.php?u=${url}`}
    >
      <div className={styles.facebook}>
        <FontAwesomeIcon
          className={styles.icon}
          icon={faFacebookF}
        ></FontAwesomeIcon>
      </div>
    </a>
  );
};

export const Twitter = ({ url }) => {
  return (
    <a
      target="_blank"
      rel="noopener noreferrer"
      href={`https://twitter.com/intent/tweet?url=${url}`}
    >
      <div className={styles.twitter}>
        <FontAwesomeIcon
          className={styles.icon}
          icon={faTwitter}
        ></FontAwesomeIcon>
      </div>
    </a>
  );
};

export const LinkedIn = ({ url }) => {
  return (
    <a
      target="_blank"
      rel="noopener noreferrer"
      href={`https://www.linkedin.com/shareArticle?mini=true&url=${url}&source=https%3A%2F%2Fcollaction.org`}
    >
      <div className={styles.linkedin}>
        <FontAwesomeIcon
          className={styles.icon}
          icon={faLinkedinIn}
        ></FontAwesomeIcon>
      </div>
    </a>
  );
};

export const Email = ({
  subject,
  body,
}: {
  subject: string;
  body?: string;
}) => {
  return (
    <a
      target="_blank"
      rel="noopener noreferrer"
      href={`mailto:?subject=${subject}&body=${body || ""}`}
    >
      <div className={styles.email}>
        <FontAwesomeIcon className={styles.icon} icon={faAt}></FontAwesomeIcon>
      </div>
    </a>
  );
};
