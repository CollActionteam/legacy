import React from "react";
import Arrow from "../../../assets/step3.png";
import styles from "./Kickstart.module.scss";
import LazyImage from "../../../components/LazyImage/LazyImage";

const Kickstart = () => {
  return (
    <div className={styles.kickstart}>
      <div>
        <h2 className={styles.title}>Kickstart your crowdaction idea on CollAction</h2>
        <p>
          Want to discuss your crowdaction idea with the CollAction team? Send us an email at <a href="mailto:hello@collaction.org">hello@collaction.org</a>.
        </p>
      </div>
      <LazyImage className={styles.image} src={Arrow} alt="arrow" />
    </div>
  );
}

export default Kickstart;