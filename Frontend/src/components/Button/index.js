import React from "react";
import { Link } from "gatsby";
import styles from "./style.module.scss";

export default ({ to, children }) => (
  <Link to={to}>
    <button className={styles.button}>
        {children}
    </button>
  </Link>
);