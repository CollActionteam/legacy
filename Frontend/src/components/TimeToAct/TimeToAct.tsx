import React from "react";
import { GhostButton } from "../Button/Button";
import styles from './TimeToAct.module.scss';
import { useHistory } from "react-router-dom";

export default () => {
    const history = useHistory();

    return <React.Fragment>
        <section className={styles.timeToActStep}>
            <h4 className={styles.timeToActTitle}>Idea</h4>
            <div className={styles.timeToActStepBody}>
                Propose a collective action and set a target number of participants.
            </div>
            <GhostButton onClick={() => history.push("/projects/start")}>Start crowdaction</GhostButton>
        </section>
        <section className={styles.timeToActStep}>
            <h4 className={styles.timeToActTitle}>Crowd</h4>
            <div className={styles.timeToActStepBody}>
                People pledge to take action if the target is met before the deadline.
            </div>
            <GhostButton onClick={() => history.push("/projects/find")}>Find crowdaction</GhostButton>
        </section>
        <section className={styles.timeToActStep}>
            <h4 className={styles.timeToActTitle}>Action</h4>
            <div className={styles.timeToActStepBody}>
                If enough people commit, we all act!
            </div>
            <GhostButton onClick={() => history.push("/about#faq")}>Read more</GhostButton>
        </section>
    </React.Fragment>;
};