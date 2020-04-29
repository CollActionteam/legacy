import React from "react";
import Arrow from "../../../assets/step3.png";
import { LazyLoadImage } from 'react-lazy-load-image-component';
import styles from "./Kickstart.module.scss";

const Kickstart = () => {
  return (
    <div className={styles.kickstart}>
      <div>
        <h2 className={styles.title}>Kickstart your project idea on CollAction</h2>
        <p>
          Want to discuss your project idea with the CollAction team? Send us an email at <a href="mailto:hello@collaction.org">hello@collaction.org</a>.
        </p>
      </div>
      <LazyLoadImage className={styles.image} src={Arrow} alt="arrow" />
    </div>
  );
}

export default Kickstart;