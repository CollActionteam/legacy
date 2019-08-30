import React from "react";
import styles from "./style.module.scss";

export const Banner = ({ children, photo, style }) => {
  const bannerStyle = {
    ...style,
    backgroundImage: `url(${photo})`
  };

  return (
    <div className={styles.banner} style={ bannerStyle }>
      {children}
    </div>
  )
};
