import React from "react";
import {siteData} from "../../api/site";

import {IProject} from "../../api/types";
import styles from "../SocialMedia/SocialMedia.module.scss";
import {IconButton} from "../Button/Button";

export default ({project}: { project: IProject }) => {

    const projectUrl = "https://www.collaction.org" + project.url;

    const socialMedia = siteData.socialMedia
        .filter((platform: any) => platform.shareUrl)
        .map((platform: any) => {
            return {
                ...platform,
                url: platform.shareUrl(projectUrl,
                    project.name)
            }
        });

    const emailBody = `Hi! I am participating in the '${project.name}' project on CollAction - do you want to join too? ` +
    `Together we make waves! You can find more information on ${projectUrl}`;
    const emailLink = `mailto:?subject=${encodeURIComponent(project.name)}&body=${encodeURIComponent(emailBody)}`;

    return <>
        <ul className={styles.list}>
            {socialMedia.map((item: any, index: number) => (
                <li key={index} className={styles.listItem}>
                    <a href={item.url} target="_blank" rel="noopener noreferrer" >
                    <IconButton aria-label={`Share on ${item.name}`} icon={["fab", item.icon]}/>
                    </a>
                </li>
            ))}
            <li className={styles.listItem}>
                <a href={emailLink} target="_blank" rel="noopener noreferrer" >
                    <IconButton aria-label={`Share by email`} icon={["far", "envelope"]}/>
                </a>
            </li>
        </ul>

    </>;
};