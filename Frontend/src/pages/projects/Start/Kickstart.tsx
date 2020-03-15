
import React from "react";

import Arrow from "../../../assets/step3.png";

import styles from "./Kickstart.module.scss";

const Kickstart = () => {
  return (
    <div className={styles.kickstart}>
      <div>
        <h2 className={styles.title}>Kickstart your project idea on CollAction</h2>
        <p>
          Want to discuss your project idea with the CollAction team? Send us an email at <a href="mailto:collactionteam@gmail.com">collactionteam@gmail.com</a>.
        </p>
      </div>
      <img className={styles.image} src={Arrow} alt=""></img>
    </div>
  );
}

export default Kickstart;