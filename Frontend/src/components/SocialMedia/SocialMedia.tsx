import React from "react";
import { IconButton } from "../Button/Button";

import styles from "./SocialMedia.module.scss";

export default ({ socialMedia } : any) => {
  return (
    <ul className={styles.list}>
      {socialMedia.map((item: any, index: number) => (
        <li key={index} className={styles.listItem}>
          <IconButton url={item.url} icon={["fab", item.icon]} />
        </li>
      ))}
    </ul>
  );
};