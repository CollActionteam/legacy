import React from 'react';
import { EmailIcon, EmailShareButton, FacebookIcon, FacebookShareButton, LinkedinIcon, LinkedinShareButton, TumblrIcon, TumblrShareButton, TwitterIcon, TwitterShareButton, WhatsappIcon, WhatsappShareButton } from 'react-share';
import { ICrowdaction } from '../../../api/types';
import styles from './SocialMediaSharing.module.scss';


interface ISocialMediaSharingProps {
  crowdaction: ICrowdaction;
}

const SocialMediaSharing = ({ crowdaction }: ISocialMediaSharingProps) => {
  const shareUrl = window.location.href;

  return (
    <div className={styles.shareButtons}>
      <FacebookShareButton quote={`${crowdaction.name}: ${crowdaction.proposal}`} hashtag={crowdaction.tags.map(tag => `#${tag.tag.name}`).join(' ')} url={shareUrl}>
        <FacebookIcon size={32} round />
      </FacebookShareButton>

      <TwitterShareButton url={shareUrl} title={crowdaction.name} hashtags={crowdaction.tags.map(tag => `#${tag.tag.name}`)}>
        <TwitterIcon size={32} round />
      </TwitterShareButton>

      <LinkedinShareButton url={shareUrl} title={crowdaction.name} summary={crowdaction.proposal}>
        <LinkedinIcon size={32} round />
      </LinkedinShareButton>

      <TumblrShareButton url={shareUrl} title={crowdaction.name} caption={crowdaction.proposal} tags={crowdaction.tags.map(tag => tag.tag.name)}>
        <TumblrIcon size={32} round />        
      </TumblrShareButton>

      <EmailShareButton url={shareUrl} subject={crowdaction.name} body="Hi! Check out this crowdaction: ">
        <EmailIcon size={32} round />
      </EmailShareButton>

      <WhatsappShareButton url={shareUrl} title={`Hi! Check out this crowdaction: ${crowdaction.name}`}>
        <WhatsappIcon size={32} round />
      </WhatsappShareButton>
    </div>
  );
}

export default SocialMediaSharing;
