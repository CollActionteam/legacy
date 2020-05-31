import React from "react";
import styles from "./DonationCard.module.scss";

const DonationThankYouCard = () => <div className={styles.card}>
    <h2>Thank you for your donation!</h2>
    <p className={styles.donationSmiley}>
        (*^‿^*)
    </p>
    <p>Thank you so much for contributing to our platform and helping us making the world a better place!</p>
</div>;

export default DonationThankYouCard;