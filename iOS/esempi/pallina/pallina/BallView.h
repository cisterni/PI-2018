//
//  BallView.h
//  pallina
//
//  Created by Davide Morelli on 27/11/2018.
//  Copyright © 2018 BioBeats. All rights reserved.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface BallView : UIView
@property float x;
@property float y;
@property float dx;
@property float dy;
@property NSTimer *timer;

- (void) setupWithXPosition: (float)x andYPosition:(float)y;
- (void) setupWithXPosition: (float)x YPosition: (float) y XVelocity: (float) dx andYVelocity: (float) dy;

- (void) throwIt;

@end

NS_ASSUME_NONNULL_END
