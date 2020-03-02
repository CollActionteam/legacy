import React from "react";
import styles from "./style.module.scss";
import { IconButton } from "../Button/Button";

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