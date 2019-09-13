import React from "react";
import styles from "./style.module.scss";

export const Banner = ({ children, photo }) => {
  return (
    <div className={styles.banner} style={{ backgroundImage: `url(${photo})` }}>
      {children}
    </div>
  )
};
