import React from "react";
import { useTranslation } from 'react-i18next';
import { GhostButton } from "../Button/Button";
import styles from "./TimeToAct.module.scss";

const TimeToAct = () => {
  const { t } = useTranslation();
  
  const step1Class = `${styles.step} ${styles.step1}`;
  const step2Class = `${styles.step} ${styles.step2}`;
  const step3Class = `${styles.step} ${styles.step3}`;
  
  return (
    <div className={styles.container}>
      <section className={step1Class}>
        <h4 className={styles.stepTitle}>{t("home.timeToAct.steps.step1.title")}</h4>
        <div className={styles.stepBody}>
          {t("home.timeToAct.steps.step1.text")}
        </div>
        <GhostButton to="/crowdactions/start">
          {t("home.timeToAct.steps.step1.button")}
        </GhostButton>
      </section>
      <section className={step2Class}>
        <h4 className={styles.stepTitle}>{t("home.timeToAct.steps.step2.title")}</h4>
        <div className={styles.stepBody}>
          {t("home.timeToAct.steps.step2.text")}
        </div>
        <GhostButton to="/crowdactions/find">
          {t("home.timeToAct.steps.step2.button")}
        </GhostButton>
      </section>
      <section className={step3Class}>
        <h4 className={styles.stepTitle}>{t("home.timeToAct.steps.step3.title")}</h4>
        <div className={styles.stepBody}>
          {t("home.timeToAct.steps.step3.text")}
        </div>
        <GhostButton to="/about#faq">
          {t("home.timeToAct.steps.step3.button")}
        </GhostButton>
      </section>
    </div>
  );
};

export default TimeToAct;